BEGIN
	-- configure email server (see https://docs.oracle.com/cd/E18283_01/server.112/e17120/schedadmin001.htm)
    DBMS_SCHEDULER.set_scheduler_attribute('email_server', 'localhost:25');
    DBMS_SCHEDULER.set_scheduler_attribute('email_server_encryption', 'STARTTLS');
    DBMS_SCHEDULER.set_scheduler_attribute('email_sender', 'nw@ufl.edu');
END;
/

/*
BEGIN
	-- drop sync job
    DBMS_SCHEDULER.DROP_JOB(job_name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"',
                                defer => false,
                                force => false);
END;
/
*/

BEGIN
	-- create and configure sync job
    DBMS_SCHEDULER.CREATE_JOB (
            job_name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"',
            job_type => 'STORED_PROCEDURE',
            job_action => 'S4_FLAT.S4_SYNC_WITH_WAREHOUSE',
            number_of_arguments => 1,
            start_date => NULL,
            repeat_interval => 'FREQ=DAILY;BYTIME=060000',
            end_date => NULL,
            enabled => FALSE,
            auto_drop => FALSE,
            comments => '');

    DBMS_SCHEDULER.SET_JOB_ARGUMENT_VALUE( 
             job_name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"', 
             argument_position => 1, 
             argument_value => '2');

    DBMS_SCHEDULER.SET_ATTRIBUTE( 
             name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"', 
             attribute => 'store_output', value => TRUE);
    DBMS_SCHEDULER.SET_ATTRIBUTE( 
             name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"', 
             attribute => 'logging_level', value => DBMS_SCHEDULER.LOGGING_OFF);

	DBMS_SCHEDULER.ADD_JOB_EMAIL_NOTIFICATION (    
             job_name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"', 
             recipients => 'nw@ufl.edu',
             sender => NULL,
             subject => 'Oracle Scheduler Job Notification - %job_owner%.%job_name%.%job_subname% %event_type%',
             body => 'Job: %job_owner%.%job_name%.%job_subname%
Event: %event_type%
Date: %event_timestamp%
Log id: %log_id%
Job class: %job_class_name%
Run count: %run_count%
Failure count: %failure_count%
Retry count: %retry_count%
Error code: %error_code%
Error message: %error_message%',
             events => 'job_broken, job_chain_stalled, job_failed, job_over_max_dur, job_sch_lim_reached, job_succeeded',
             	filter_condition => NULL 
             );
    DBMS_SCHEDULER.enable(
             name => '"S4_FLAT"."S4_SYNC_WITH_WAREHOUSE_JOB"');
END;
/
