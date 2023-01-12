# NovelEnvironments



## File storage

All data is stored in folders that are in the folder defined by 'Application.dataPath'.

### Logging
The logging data is stored in the folder 'ExperimentLogs_{participant_id}'.

The movement data of the player is stored in the file 'movement.csv'
- The format is `{environment_id};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z}`
- This data is logged X times per second.

The task data is stored in the file 'tasks.csv'
- The format is `{environment_id};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z};{task}`
- When a picture is taken the location of the player is stored.
- When a Gatherable is gathered the location of the Gatherable is stored.
- When looking at a landmark the landmark position is stored.

### Picture storage
The pictures are stored in the folder 'Pictures'
- All pictures are stored as .png
