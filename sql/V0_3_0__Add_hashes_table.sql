create table hashes (
	`key` varchar(100) not null,
	`field` varchar(100) not null,
	`value` TEXT null,

	PRIMARY KEY (`key`, `field`)
);