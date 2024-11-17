alter table `options`
    add column deleted boolean default 0,
    add column deletedDate datetime default null,
    add column activeName VARCHAR(40) AS (IF(deleted = 0, name, NULL)),
    drop index `idx_options_name`,
    add unique `idx_options_name` (`groupId`, `activeName`);

alter table `option_groups`
    add column deleted boolean default 0,
    add column deletedDate datetime default null,
    add column activeName VARCHAR(40) AS (IF(deleted = 0, name, NULL)),
    drop index `idx_optiongroups_name`,
    add unique `idx_optiongroups_name` (`roomId`, `activeName`);
