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

CREATE TABLE `locks` (
  `lockKey` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`lockKey`)
);

create table users (
	`id` bigint primary key,
	`username` varchar(100) not null,
	`displayName` varchar(100) not null,
	`createdDate` datetime not null
);

CREATE TABLE runs (
	`id` bigint primary key,
	`roomId` bigint not null,
	`userId` bigint not null,
	`groupId` bigint not null,
	`startTime` datetime not null,
	`endTime` datetime default null,
	`ended` boolean default false,
	`createdDate` datetime not null
);

CREATE TABLE run_results (
	`runId` bigint primary key,
	`runnerUserId` bigint not null,
	`endedTime` datetime not null
);

CREATE TABLE rooms (
	`id` bigint primary key,
	`name` varchar(200) not null,
	`createdBy` bigint not null,
	`createdDate` datetime not null
);

create table options (
	`id` bigint primary key,
	`name` varchar(40) not null,
	`groupId` bigint not null,
	`createdBy` bigint not null,
	`createdDate` datetime not null,
	
	unique `idx_options_name` (`groupId`, `name`)
);

create table option_groups (
	`id` bigint primary key,
	`name` varchar(40) not null,
	`roomId` bigint not null,
	`createdBy` bigint not null,
	`createdDate` datetime not null,
	
	unique `idx_optiongroups_name` (`roomId`, `name`)
);