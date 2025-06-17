extends CharacterBody3D

# Columns and rows are relative to the character's Z axis
var bottom_ray_collider_columns = 6
var bottom_ray_collider_rows = 5
var bottom_ray_colliders_enabled = true

func _init():
	# BOTTOM RAY COLLIDERS
	# These are included for the following purposes
	# - Allow the character to climb staircase steps without jumping (provided that they aren't too steep)
	# - Make it easier for the character to successfully complete a long or high jump
	if bottom_ray_colliders_enabled:
		var bottom_ray_collider_area: MeshInstance3D = get_node_or_null("BottomRayColliderArea")
		if (bottom_ray_collider_area != null): generate_bottom_ray_colliders(bottom_ray_collider_area)

func generate_bottom_ray_colliders(bottom_ray_collider_area: MeshInstance3D):
	var mesh: BoxMesh = bottom_ray_collider_area.mesh
	var x_size = mesh.size.x
	var y_size = mesh.size.y
	var z_size = mesh.size.z

	var x_separation = x_size / (bottom_ray_collider_columns - 1)
	var z_separation = z_size / (bottom_ray_collider_rows - 1)

	# This is fine since the collision rays will be a sibling of the MeshInstance3D
	# containing bottom_ray_collider_area
	var ray_y_pos = bottom_ray_collider_area.position.y - y_size / 2
	var start_x_pos = bottom_ray_collider_area.position.x - x_size / 2
	var start_z_pos = bottom_ray_collider_area.position.z - z_size / 2

	var ray_shape: SeparationRayShape3D = SeparationRayShape3D.new()
	ray_shape.length = y_size
	ray_shape.slide_on_slope = false # This is the default, but we'll still specify this just in case.
	
	for i in range(bottom_ray_collider_columns):
		for j in range(bottom_ray_collider_columns):
			var collision_shape: CollisionShape3D = CollisionShape3D.new()
			collision_shape.shape = ray_shape
			collision_shape.rotation = Vector3(PI / 2, 0, 0)
			
			collision_shape.position = Vector3(
				-start_x_pos + i * x_separation,
				ray_y_pos,
				-start_z_pos + i * z_separation
			)
