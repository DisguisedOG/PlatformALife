extends SceneTree

func _init():
	var frames = ["frame_000.png", "frame_001.png", "frame_016.png"]
	var base_dir = "res://Assets/MAINUI/"
	
	for f in frames:
		var path = base_dir + f
		var img = Image.load_from_file(ProjectSettings.globalize_path(path))
		if img == null:
			print("Failed to load " + path)
			continue
			
		var rect = img.get_used_rect()
		var cropped = img.get_region(rect)
		
		var cropped_path = path.replace(".png", "_cropped.png")
		cropped.save_png(ProjectSettings.globalize_path(cropped_path))
		print(f + " cropped to " + str(rect) + " and saved as " + cropped_path)
		
	quit()
