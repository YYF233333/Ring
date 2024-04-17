extends Node2D

class_name GhostTetromino 

@onready var piece_scene = preload(Shared.TETROMINO_PATH + "Scenes/piece.tscn")
@onready var ghost_texture = preload(Shared.TETROMINO_PATH + "Assets/Ghost.png")

var tetromino_data: Resource

func _ready():
	var tetromino_cells = Shared.cells[tetromino_data.tetromino_type]
	
	for cell in tetromino_cells:
		var piece = piece_scene.instantiate() as Piece
		add_child(piece)
		piece.set_texture(ghost_texture)
		piece.position = cell * piece.get_size()

func set_ghost_tetromino(new_position: Vector2, pieces_position):
	global_position = new_position
	
	var pieces = get_children()
	for i in pieces.size():
		pieces[i].position = pieces_position[i]	 
