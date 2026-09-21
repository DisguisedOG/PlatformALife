#!/usr/bin/env bash
#
# Cloud Agent install script for the "Tales of Aotearoa" (PlatformALife) Godot 4.7 Mono project.
#
# Runs once after checkout to bootstrap a headless Linux dev environment that can:
#   * compile the C# game code with the .NET 8 SDK (`dotnet build`)
#   * import and run the Godot 4.7 Mono project (headless or on a virtual display)
#
# The script is idempotent: it can be re-run safely and only does work that is missing.
set -euo pipefail

GODOT_VERSION="4.7-stable"
GODOT_DIRNAME="Godot_v${GODOT_VERSION}_mono_linux_x86_64"
GODOT_ZIP="${GODOT_DIRNAME}.zip"
GODOT_URL="https://github.com/godotengine/godot/releases/download/${GODOT_VERSION}/${GODOT_ZIP}"
GODOT_BIN_NAME="Godot_v${GODOT_VERSION}_mono_linux.x86_64"

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "==> [1/4] Installing system packages (dotnet SDK, headless GL/Vulkan, Xvfb, tooling)"
export DEBIAN_FRONTEND=noninteractive
sudo apt-get update -qq
sudo apt-get install -y --no-install-recommends \
  dotnet-sdk-8.0 \
  unzip curl ca-certificates \
  xvfb x11-utils xdotool ffmpeg imagemagick \
  libgl1 libglx-mesa0 libgl1-mesa-dri mesa-vulkan-drivers libvulkan1 \
  libxcursor1 libxinerama1 libxrandr2 libxi6 libxkbcommon0 \
  libfontconfig1 libasound2t64 libpulse0 libudev1

echo "==> [2/4] Installing Godot ${GODOT_VERSION} (Mono/.NET) to /opt/godot"
if [ ! -x /opt/godot/godot ]; then
  tmp="$(mktemp -d)"
  curl -sSL -o "${tmp}/godot.zip" "${GODOT_URL}"
  unzip -oq "${tmp}/godot.zip" -d "${tmp}"
  sudo mkdir -p /opt/godot/GodotSharp
  sudo cp "${tmp}/${GODOT_DIRNAME}/${GODOT_BIN_NAME}" /opt/godot/godot
  sudo cp -r "${tmp}/${GODOT_DIRNAME}/GodotSharp/." /opt/godot/GodotSharp/
  sudo chmod +x /opt/godot/godot
  sudo ln -sf /opt/godot/godot /usr/local/bin/godot
  rm -rf "${tmp}"
else
  echo "    Godot already present, skipping download."
fi
godot --headless --version

echo "==> [3/4] Building C# (.NET 8) project"
cd "${REPO_ROOT}"
dotnet build

echo "==> [4/4] Importing Godot project assets (headless, first-time cache build)"
godot --headless --import

echo "==> Cloud Agent setup complete."
echo "    Build C#:            dotnet build"
echo "    Run headless:        godot --headless"
echo "    Run with rendering:  DISPLAY=:99 godot --rendering-method gl_compatibility --rendering-driver opengl3"
