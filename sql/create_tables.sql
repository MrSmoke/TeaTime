create table users (
	`id` bigint primary key,
	`name` varchar(200) not null,
	`date_created` datetime not null
);

create table rooms (
	`id` bigint primary key,
	`name` varchar(200) not null,
	`date_created` datetime not null
);

create table room_groups (
	`id` bigint primary key,
	`name` varchar(32) not null,
	`room_id` bigint not null,
	`date_created` datetime not null,
	`date_deleted` datetime default null,
	
	unique (room_id, name),
	foreign key fk_roomgroups_rooms_roomid (room_id) references rooms(id)
);

create table room_group_options (
	`id` bigint primary key,
	`name` varchar(40) not null,
	`room_group_id` bigint not null,
	`date_created` datetime not null,
	`date_deleted` datetime default null,
	
	unique (room_group_id, name),
	foreign key fk_roomgroupoptions_roomgroups_roomgroupid (room_group_id) references room_groups(id)
);

create table links (
	`link` varchar(100) not null,
	`linkType` smallint not null,
	`objectId` bigint not null,
	
	primary key (link, linkType)
);

CREATE TABLE `ids64` (
  `id` bigint(20) unsigned NOT NULL auto_increment,
  `stub` char(1) NOT NULL default '',
  PRIMARY KEY  (`id`),
  UNIQUE KEY `stub` (`stub`)
);