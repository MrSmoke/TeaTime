ALTER TABLE `links`
ADD INDEX `idx_links_linkType_objectId` (`linkType` ASC, `objectId` ASC) VISIBLE;


ALTER TABLE `runs`
ADD INDEX `idx_runs_roomId_ended_createdDate` (`roomId` ASC, `ended` ASC, `createdDate` DESC) VISIBLE;
