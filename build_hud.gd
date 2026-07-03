extends SceneTree

func _init():
	var root = CanvasLayer.new()
	root.name = "HUD"
	root.set_script(preload("res://Scripts/HUD.cs"))
	
	# --- Status Bar ---
	var status_bar = TextureRect.new()
	status_bar.name = "StatusBar"
	status_bar.texture = preload("res://Assets/MAINUI/frame_000_cropped.png")
	status_bar.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	status_bar.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
	# Actual size 903x307, let's scale it to 451x153
	status_bar.custom_minimum_size = Vector2(451, 153)
	status_bar.size = Vector2(451, 153)
	status_bar.set_anchors_preset(Control.PRESET_TOP_LEFT)
	status_bar.position = Vector2(16, 16)
	status_bar.mouse_filter = Control.MOUSE_FILTER_IGNORE
	root.add_child(status_bar)
	status_bar.owner = root
	
	var health_label = Label.new()
	health_label.name = "HealthLabel"
	health_label.text = "HP: 100/100"
	health_label.add_theme_color_override("font_color", Color(1, 1, 1))
	health_label.add_theme_font_size_override("font_size", 20)
	# Position on the red bar (approx)
	health_label.position = Vector2(230, 46)
	status_bar.add_child(health_label)
	health_label.owner = root
	
	# --- Hotbar ---
	var hotbar = TextureRect.new()
	hotbar.name = "Hotbar"
	hotbar.texture = preload("res://Assets/MAINUI/frame_016_cropped.png")
	hotbar.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	hotbar.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
	# Actual size 1113x148, let's scale it to 556x74
	hotbar.custom_minimum_size = Vector2(556, 74)
	hotbar.size = Vector2(556, 74)
	hotbar.set_anchors_preset(Control.PRESET_CENTER_BOTTOM)
	# Center it horizontally using anchor logic. If anchors are PRESET_CENTER_BOTTOM, we need to adjust position so it sits above the bottom
	hotbar.position = Vector2(1280.0/2.0 - 556.0/2.0, 720.0 - 74.0 - 16.0)
	hotbar.mouse_filter = Control.MOUSE_FILTER_IGNORE
	root.add_child(hotbar)
	hotbar.owner = root

	# --- Inventory Window ---
	var inv_panel = Control.new()
	inv_panel.name = "Panel" 
	# Actual size of the inventory image is 570x701. We'll keep it 1:1.
	inv_panel.custom_minimum_size = Vector2(570, 701)
	inv_panel.size = Vector2(570, 701)
	inv_panel.position = Vector2(1280.0/2.0 - 570.0/2.0, 720.0/2.0 - 701.0/2.0)
	root.add_child(inv_panel)
	inv_panel.owner = root
	
	var inv_bg = TextureRect.new()
	inv_bg.name = "Background"
	inv_bg.texture = preload("res://Assets/MAINUI/frame_001_cropped.png")
	inv_bg.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	inv_bg.set_anchors_preset(Control.PRESET_FULL_RECT)
	inv_panel.add_child(inv_bg)
	inv_bg.owner = root
	
	var gold = Label.new()
	gold.name = "GoldLabel"
	gold.text = "Gold: 0"
	gold.add_theme_color_override("font_color", Color(1, 0.84, 0))
	gold.add_theme_font_size_override("font_size", 24)
	# Put gold label at top right of the inventory window
	gold.position = Vector2(400, 24)
	inv_panel.add_child(gold)
	gold.owner = root
	
	var grid = GridContainer.new()
	grid.name = "GridContainer"
	grid.columns = 5
	# Position the grid carefully over the drawn slots
	grid.position = Vector2(24, 112)
	grid.size = Vector2(522, 565)
	grid.add_theme_constant_override("h_separation", 6)
	grid.add_theme_constant_override("v_separation", 6)
	inv_panel.add_child(grid)
	grid.owner = root

	var packed = PackedScene.new()
	packed.pack(root)
	ResourceSaver.save(packed, "res://Scenes/HUD.tscn")
	print("HUD rebuilt.")
	quit()
