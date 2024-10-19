extends Control

func get_message(runtimeName: String, message) -> void:
	print("Office get " + message + " from " + runtimeName)


func _on_computer_pressed(node: Node) -> void:
	get_parent().SwitchRuntime("AVG")
