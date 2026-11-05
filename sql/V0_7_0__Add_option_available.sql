alter table `options`
    add column available tinyint(1) default 1 not null after name;
