extends Node

var current_tetromino
var next_tetromino 

@onready var board = $"../Board" as Board
@onready var ui = $"../UI" as UI
var is_game_over = false

func _ready():
	current_tetromino = get_parent().block_spawn_algorithm.call(
		Shared.TetrominoType.values())	
	next_tetromino = get_parent().block_spawn_algorithm.call(
		Shared.TetrominoType.values())	
	board.spawn_tetromino(current_tetromino, false, null)
	board.spawn_tetromino(next_tetromino, true, Vector2(100, 50))
	board.tetromino_locked.connect(on_tetromino_locked)
	board.game_over.connect(on_game_over)
	
func on_tetromino_locked():
	if is_game_over:
		return
	current_tetromino = next_tetromino
	next_tetromino = get_parent().block_spawn_algorithm.call(
		Shared.TetrominoType.values())
	board.spawn_tetromino(current_tetromino, false, null)
	board.spawn_tetromino(next_tetromino, true, Vector2(100, 50))

func on_game_over():
	is_game_over = true
	ui.show_game_over()
	board.process_mode = Node.PROCESS_MODE_DISABLED
