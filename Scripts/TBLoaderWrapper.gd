@tool
extends Node3D
class_name TBLoaderWrapper

var _tbLoader: TBLoader = TBLoader.new()

@export var refresh: bool:
    set(val):
        _refresh()

@export_category("Map")
@export_file var map_resource: String:
    set(val):
        map_resource = val
        _refresh()
@export var map_inverse_scale: float = 16:
    set(val):
        map_inverse_scale = val
        _refresh()

@export_category("Entities")
@export var entity_common: bool = true:
    set(val):
        entity_common = val
        _refresh()
@export_dir var entity_path: String = "res://entities":
    set(val):
        entity_path = val
        _refresh()

@export_category("Textures")
@export_dir var texture_path: String = "res://textures":
    set(val):
        texture_path = val
        _refresh()

func _ready():
    add_child(_tbLoader)
    _refresh()

func _refresh():
    _tbLoader.map_resource = map_resource
    _tbLoader.map_inverse_scale = map_inverse_scale
    _tbLoader.entity_common = entity_common
    _tbLoader.entity_path = entity_path
    _tbLoader.texture_path = texture_path
    
    _tbLoader.build_meshes()
