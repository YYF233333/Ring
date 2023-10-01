class_name RingAnimation
## Animation base class

var _type: AnimationType

## Lambda func which takes a Tween and configure it.
var apply: Callable # func(Tween) -> Tween
