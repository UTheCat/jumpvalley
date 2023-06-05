## Represents a single song file with some metadata attached to it
class_name Song

## The audio stream itself
var stream: AudioStreamOggVorbis

## The file path to the song
var file_path: String = ""

## The name of the song
var name: String = ""

## The artists that made the song
var artists: String = ""

## The album the song belongs to
var album: String = ""

func _init(init_path, init_name, init_artists, init_album):
    file_path = init_path
    name = init_name
    artists = init_artists
    album = init_album

## Creates an [code]AudioStream[/code] for the specified [code]file_path[/code]
func open():
    stream = AudioStreamOggVorbis.new()
    stream.resource_path = file_path
    stream.loop = true

## Prepares the audio stream for garbage collection
## Call this to free up memory when an AudioStream is no longer needed
func close():
    stream = null
