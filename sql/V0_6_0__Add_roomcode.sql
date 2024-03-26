alter table rooms
    add roomCode char(24) null,
    add unique `idx_rooms_roomcode` (`roomCode`);

