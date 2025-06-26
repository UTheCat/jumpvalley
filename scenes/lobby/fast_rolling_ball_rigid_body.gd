extends RigidBody3D

const DESTROY_BELOW_Y = -250.0;

func _physics_process(_delta):
	if (position.y < DESTROY_BELOW_Y):
		set_physics_process(false)
		queue_free()
