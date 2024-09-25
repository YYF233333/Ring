extends Control

signal pressed(node: Node)

@onready var texture_button: TextureButton = $TextureButton
@onready var animation_player: AnimationPlayer = $AnimationPlayer


func _ready() -> void:
	texture_button.pivot_offset = texture_button.size / 2.0
	texture_button.texture_click_mask.create_from_image_alpha(texture_button.texture_normal.get_image())


#func _on_texture_button_focus_entered() -> void:
	#animation_player.play("enter")
	
	
func _on_texture_button_mouse_entered() -> void:
	animation_player.play("enter")


#func _on_texture_button_focus_exited() -> void:
	#animation_player.play("exit")


func _on_texture_button_mouse_exited() -> void:
	animation_player.play("exit")
	

func _on_texture_button_button_down() -> void:
	if not texture_button.is_hovered():
		return
	texture_button.set("shader_parameter/width", 3)
	animation_player.play("down")
	

func _on_texture_button_button_up() -> void:
	if not texture_button.is_hovered():
		return
	texture_button.set("shader_parameter/width", 3)
	animation_player.play("up")
	

func _on_texture_button_pressed() -> void:
	pressed.emit(self)
	
	
# debug
#func _on_animation_player_animation_changed(old_name: StringName, new_name: StringName) -> void:
	#print("animation_changed: " + old_name + " -> " + new_name)

# debug
#func _on_animation_player_current_animation_changed(name: String) -> void:
	#print("current_animation_changed_to: " + name)
