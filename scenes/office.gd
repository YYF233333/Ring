extends Control

func get_message(runtimeName: String, message) -> void:
	print_debug("Office get " + message + " from " + runtimeName)

func _on_computer_tb_pressed(node: Node) -> void:
	await get_tree().create_timer(0.5).timeout
	get_parent().call_deferred("SwitchRuntime", "AVG", "", "Pause")
