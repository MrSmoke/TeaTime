ALTER TABLE `illmakes`
    ADD CONSTRAINT `fk_illmakes_runId_runs_id`
        FOREIGN KEY (`runId`)
            REFERENCES `runs` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION,
    ADD CONSTRAINT `fk_illmakes_userId_users_id`
        FOREIGN KEY (`userId`)
            REFERENCES `users` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION;

ALTER TABLE `option_groups`
    ADD CONSTRAINT `fk_option_groups_roomId_rooms_id`
        FOREIGN KEY (`roomId`)
            REFERENCES `rooms` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION;

ALTER TABLE `options`
    ADD CONSTRAINT `fk_options_groupId_option_groups_id`
        FOREIGN KEY (`groupId`)
            REFERENCES `option_groups` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION;

ALTER TABLE `orders`
    ADD CONSTRAINT `fk_orders_runId_runs_id`
        FOREIGN KEY (`runId`)
            REFERENCES `runs` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION,
    ADD CONSTRAINT `fk_orders_userId_users_id`
        FOREIGN KEY (`userId`)
            REFERENCES `users` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION,
    ADD CONSTRAINT `fk_orders_optionId_options_id`
        FOREIGN KEY (`optionId`)
            REFERENCES `options` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION;

ALTER TABLE `run_results`
    ADD CONSTRAINT `fk_run_results_runnerUserId_users_id`
        FOREIGN KEY (`runnerUserId`)
            REFERENCES `users` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION;

ALTER TABLE `runs`
    ADD CONSTRAINT `fk_runs_roomId_rooms_id`
        FOREIGN KEY (`roomId`)
            REFERENCES `rooms` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION,
    ADD CONSTRAINT `fk_runs_userId_users_id`
        FOREIGN KEY (`userId`)
            REFERENCES `users` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION,
    ADD CONSTRAINT `fk_runs_groupId_options_groups_id`
        FOREIGN KEY (`groupId`)
            REFERENCES `option_groups` (`id`)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION;