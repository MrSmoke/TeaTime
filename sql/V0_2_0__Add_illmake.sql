create table illmakes (
	`id` bigint primary key,
	`runId` bigint not null,
	`userId` bigint not null,
	`createdDate` datetime not null
);