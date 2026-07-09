import os

tscn_content = """[gd_scene load_steps=10 format=3 uid="uid://dm6gwc6ehujoc"]

[ext_resource type="Script" path="res://Scripts/GameplayHUD.cs" id="1_ghud"]
[ext_resource type="Texture2D" path="res://assets/mainui/MAIN_UI_FINAL.png" id="2_sheet"]

[sub_resource type="AtlasTexture" id="AtlasTexture_panel"]
atlas = ExtResource("2_sheet")
region = Rect2(0, 55, 1024, 145)

[sub_resource type="AtlasTexture" id="AtlasTexture_hp_bg"]
atlas = ExtResource("2_sheet")
region = Rect2(0, 128, 128, 32)

[sub_resource type="AtlasTexture" id="AtlasTexture_hp_fill"]
atlas = ExtResource("2_sheet")
region = Rect2(128, 128, 128, 32)

[sub_resource type="AtlasTexture" id="AtlasTexture_mp_bg"]
atlas = ExtResource("2_sheet")
region = Rect2(0, 160, 128, 32)

[sub_resource type="AtlasTexture" id="AtlasTexture_mp_fill"]
atlas = ExtResource("2_sheet")
region = Rect2(128, 160, 128, 32)

[sub_resource type="AtlasTexture" id="AtlasTexture_exp_bg"]
atlas = ExtResource("2_sheet")
region = Rect2(0, 192, 128, 32)

[sub_resource type="AtlasTexture" id="AtlasTexture_exp_fill"]
atlas = ExtResource("2_sheet")
region = Rect2(128, 192, 128, 32)

[node name="GameplayHUD" type="CanvasLayer"]
script = ExtResource("1_ghud")

[node name="HUDControl" type="Control" parent="."]
layout_mode = 3
anchors_preset = 12
anchor_top = 1.0
anchor_right = 1.0
anchor_bottom = 1.0
offset_top = -72.0
offset_bottom = 0.0
grow_horizontal = 2
grow_vertical = 0
custom_minimum_size = Vector2(0, 72)
mouse_filter = 2

[node name="MainPanel" type="NinePatchRect" parent="HUDControl"]
layout_mode = 1
anchors_preset = 15
anchor_right = 1.0
anchor_bottom = 1.0
grow_horizontal = 2
grow_vertical = 2
texture = SubResource("AtlasTexture_panel")
patch_margin_left = 16
patch_margin_top = 16
patch_margin_right = 16
patch_margin_bottom = 16

[node name="Margin" type="MarginContainer" parent="HUDControl/MainPanel"]
layout_mode = 1
anchors_preset = 15
anchor_right = 1.0
anchor_bottom = 1.0
grow_horizontal = 2
grow_vertical = 2
theme_override_constants/margin_left = 16
theme_override_constants/margin_top = 16
theme_override_constants/margin_right = 16
theme_override_constants/margin_bottom = 16

[node name="VBox" type="VBoxContainer" parent="HUDControl/MainPanel/Margin"]
layout_mode = 2
theme_override_constants/separation = 4

[node name="HBox" type="HBoxContainer" parent="HUDControl/MainPanel/Margin/VBox"]
layout_mode = 2
theme_override_constants/separation = 32

[node name="CharacterNameLabel" type="Label" parent="HUDControl/MainPanel/Margin/VBox/HBox"]
layout_mode = 2
theme_override_colors/font_color = Color(0.81, 0.71, 0.23, 1)
theme_override_colors/font_shadow_color = Color(0, 0, 0, 1)
text = "PlayerName"
horizontal_alignment = 1

[node name="GoldLabel" type="Label" parent="HUDControl/MainPanel/Margin/VBox/HBox"]
layout_mode = 2
theme_override_colors/font_color = Color(1, 1, 1, 1)
theme_override_colors/font_shadow_color = Color(0, 0, 0, 1)
text = "Gold: 0"
horizontal_alignment = 1

[node name="HPRow" type="HBoxContainer" parent="HUDControl/MainPanel/Margin/VBox"]
layout_mode = 2

[node name="HPLabel" type="Label" parent="HUDControl/MainPanel/Margin/VBox/HPRow"]
custom_minimum_size = Vector2(40, 0)
layout_mode = 2
text = "HP:"

[node name="HPBar" type="TextureProgressBar" parent="HUDControl/MainPanel/Margin/VBox/HPRow"]
layout_mode = 2
size_flags_horizontal = 3
size_flags_vertical = 4
value = 100.0
nine_patch_stretch = true
texture_under = SubResource("AtlasTexture_hp_bg")
texture_progress = SubResource("AtlasTexture_hp_fill")

[node name="MPRow" type="HBoxContainer" parent="HUDControl/MainPanel/Margin/VBox"]
layout_mode = 2

[node name="MPLabel" type="Label" parent="HUDControl/MainPanel/Margin/VBox/MPRow"]
custom_minimum_size = Vector2(40, 0)
layout_mode = 2
text = "MP:"

[node name="MPBar" type="TextureProgressBar" parent="HUDControl/MainPanel/Margin/VBox/MPRow"]
layout_mode = 2
size_flags_horizontal = 3
size_flags_vertical = 4
value = 100.0
nine_patch_stretch = true
texture_under = SubResource("AtlasTexture_mp_bg")
texture_progress = SubResource("AtlasTexture_mp_fill")

[node name="ExpRow" type="HBoxContainer" parent="HUDControl/MainPanel/Margin/VBox"]
layout_mode = 2

[node name="EXPLabel" type="Label" parent="HUDControl/MainPanel/Margin/VBox/ExpRow"]
custom_minimum_size = Vector2(40, 0)
layout_mode = 2
text = "EXP:"

[node name="ExpBar" type="TextureProgressBar" parent="HUDControl/MainPanel/Margin/VBox/ExpRow"]
layout_mode = 2
size_flags_horizontal = 3
size_flags_vertical = 4
value = 50.0
nine_patch_stretch = true
texture_under = SubResource("AtlasTexture_exp_bg")
texture_progress = SubResource("AtlasTexture_exp_fill")
"""

with open(r"d:\KiwiKing Studios\PlatformALife\Scenes\GameplayHUD.tscn", "w") as f:
    f.write(tscn_content)
