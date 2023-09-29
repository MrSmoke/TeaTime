alter table `options`
    add column deleted boolean default 0,
    add column deletedDate datetime default null,
    drop index `idx_options_name`,
    add unique `idx_options_name` (`groupId`, `name`, (IF(deleted,null,1)));

alter table `option_groups`
    add column deleted boolean default 0,
    add column deletedDate datetime default null,
    drop index `idx_optiongroups_name`,
    unique `idx_optiongroups_name` (`roomId`, `name`, (IF(deleted,null,1)));
