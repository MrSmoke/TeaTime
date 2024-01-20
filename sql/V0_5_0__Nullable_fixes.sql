alter table option_groups
    modify deleted tinyint(1) default 0 not null;

alter table options
    modify deleted tinyint(1) default 0 not null;

alter table runs
    modify ended tinyint(1) default 0 not null;
