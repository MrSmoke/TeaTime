-- insert missing option groups
insert into option_groups (id, name, roomId, createdBy, createdDate, deleted, deletedDate)
select distinct groupId, concat('deleted_', groupid), roomId, 0, utc_timestamp, true, utc_timestamp from runs
where groupId not in (select id from option_groups);

-- Delete options that have no run info (we cant get the group id)
delete FROM `options` where groupId not in (select id from option_groups);

-- Create missing options
insert into options (id, name, groupId, createdBy, createdDate, deleted, deletedDate)
select distinct o.optionId, concat('deleted_', o.optionId),r.groupId, 0, utc_timestamp, true, utc_timestamp FROM orders o
join runs r on o.runId = r.id
where optionId not in (select id from options);
