extends CharacterBody3D

# Columns and rows are relative to the character's Z axis
var bottom_ray_collider_columns = 3
var bottom_ray_collider_rows = 3
var bottom_ray

func _init():
	# BOTTOM RAY COLLIDERS
	# These are included for the following purposes
	# - Allow the character to climb staircase steps without jumping (provided that they aren't too steep)
	# - Make it easier for the character to successfully complete a long or high jump
	pass
