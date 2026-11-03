alter table `orders`
    add column deleted tinyint(1) default 0 not null,
    add column deletedDate datetime default null;
