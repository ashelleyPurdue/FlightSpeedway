# Why is this written in GDScript instead of C#?
# Because Godot 4.1's editor has a weird bug where, in some circumstances, C#
# [Tool] scripts will cause the editor lose its goddamn mind and forget how to
# read C#, resulting in crazy things that I can't even describe.
#
# I don't know exactly what circumstances cause this, but whatever they were,
# I was triggering them.  GDScript @tool scripts don't trigger this bug, so
# here we are.
@tool
class_name RingPath
extends Path3D

@export var RingPrefab: PackedScene:
    set(val):
        RingPrefab = val
        Refresh()

@export var RingCount: int = 2:
    set(val):
        RingCount = val
        Refresh()

func _ready():
    connect("curve_changed", Callable(self, "Refresh"))
    Refresh()

func Refresh():
    while (get_child_count() > 0):
        var child = get_child(0)
        remove_child(child)
        child.queue_free()

    if (RingPrefab == null):
        return

    if (RingCount < 2):
        return

    var length = curve.get_baked_length()
    var interval = length / (RingCount - 1)

    for i in range(0, RingCount):
        CreateRing(curve.sample_baked_with_rotation(interval * i))

func CreateRing(tf: Transform3D):
    var node: Node3D = RingPrefab.instantiate()
    node.transform = tf
    add_child(node)