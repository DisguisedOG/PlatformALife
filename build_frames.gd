extends SceneTree

func _init():
	var frames = SpriteFrames.new()
	var base_path = "res://Assets/CharacterBases/Male/Male_Base01/frame_%03d.png"

	frames.add_animation("idle")
	frames.set_animation_loop("idle", true)
	frames.set_animation_speed("idle", 10.0)
	
	frames.add_animation("walk")
	frames.set_animation_loop("walk", true)
	frames.set_animation_speed("walk", 12.0)
	
	frames.add_animation("jump")
	frames.set_animation_loop("jump", false)
	frames.set_animation_speed("jump", 10.0)
	
	frames.add_animation("climb")
	frames.set_animation_loop("climb", true)
	frames.set_animation_speed("climb", 8.0)
	
	for i in range(30):
		var tex_path = base_path % i
		var tex = load(tex_path)
		if tex != null:
			if i < 10:
				frames.add_frame("idle", tex)
			elif i < 20:
				frames.add_frame("walk", tex)
			elif i < 25:
				frames.add_frame("jump", tex)
			else:
				frames.add_frame("climb", tex)

	var save_path = "res://Assets/CharacterBases/Male/MaleBase01_Frames.tres"
	var err = ResourceSaver.save(frames, save_path)
	if err == OK:
		print("Successfully saved SpriteFrames to ", save_path)
	else:
		print("Failed to save SpriteFrames, error code: ", err)

	quit()
