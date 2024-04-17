extends Node

# 小游戏可调整参数

# 方块下落的时间间隔（单位/s）
var block_fall_timestep:
	get:
		return $Board/TetrominoType/Timer.wait_time
	set(value):
		$Board/TetrominoType/Timer.start(value)


# 方块生成策略
var block_spawn_algorithm: Callable = func(array: Array):
	return array.pick_random()

var cleanup_hook: Callable
