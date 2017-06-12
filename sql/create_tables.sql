create table users (
	`id` char(36) primary key,
	`name` varchar(200) not null,
	`date_created` datetime not null
);

create table rooms (
	`id` char(36) primary key,
	`name` varchar(200) not null,
	`date_created` datetime not null
);

create table room_groups (
	`id` char(36) primary key,
	`name` varchar(32) not null,
	`room_id` char(36) not null,
	`date_created` datetime not null,
	`date_deleted` datetime default null,
	
	unique (roomId, name),
	foreign key fk_roomgroups_rooms_roomid (roomId) references rooms(id)
);

create table room_group_options (
	`id` char(36) primary key,
	`name` varchar(40) not null,
	`room_group_id` char(36) not null,
	`date_created` datetime not null,
	`date_deleted` datetime default null,
	
	unique (room_group_id, name),
	foreign key fk_roomgroupoptions_roomgroups_roomgroupid (room_group_id) references room_groups(id)
);

create table links (
	`link` varchar(100) not null,
	`linkType` smallint not null,
	`objectId` char(36) not null,
	
	primary key (link, linkType)
);