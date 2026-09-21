#!/usr/bin/env bash
#
# Cloud Agent start script.
#
# Runs on every boot. Brings up a virtual X display (:99) with software
# OpenGL/Vulkan so the Godot game can be run and screen-captured on a
# GPU-less VM (e.g. for GUI testing and demo recordings). Idempotent.
set -euo pipefail

XVFB_DISPLAY=":99"

if pgrep -x Xvfb >/dev/null 2>&1; then
  echo "Xvfb already running."
else
  Xvfb "${XVFB_DISPLAY}" -screen 0 1920x1080x24 -ac +extension GLX +render -noreset \
    >/tmp/xvfb.log 2>&1 &
fi

# Wait for the display to accept connections.
for _ in $(seq 1 20); do
  if DISPLAY="${XVFB_DISPLAY}" xdpyinfo >/dev/null 2>&1; then
    echo "Xvfb ready on ${XVFB_DISPLAY}."
    exit 0
  fi
  sleep 0.5
done

echo "WARNING: Xvfb did not become ready on ${XVFB_DISPLAY}; see /tmp/xvfb.log" >&2
exit 0
