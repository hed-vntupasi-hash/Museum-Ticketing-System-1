USE museum_ticketing_system;

-- **********************************************

SET GLOBAL log_bin_trust_function_creators = 1;

DELIMITER //
CREATE FUNCTION ValidateTicketDate(qrcode varchar(50)) RETURNS BOOLEAN
NOT DETERMINISTIC
BEGIN
	DECLARE event_id_i INT;
    
	DECLARE startStamp TIMESTAMP;
	DECLARE endStamp TIMESTAMP;
    
    DECLARE valid BOOLEAN;
    
	SELECT E.event_id
    INTO event_id_i
    FROM events AS E
    LEFT JOIN tickets AS T
    ON E.event_id = T.event_id
    WHERE qrcode = qrcode
    LIMIT 1;
    
	SELECT
		TIMESTAMP(start_date, '00:00:00'),
		TIMESTAMP(end_date, '23:59:59')
    INTO startStamp, endStamp
    FROM events
    WHERE event_id = event_id_i
    LIMIT 1;
    
	SELECT (COUNT(*) > 0) AS count
    INTO valid
	FROM events
	WHERE
		event_id = event_id_i AND
		CURRENT_TIMESTAMP
			BETWEEN startStamp AND endStamp;
            
	RETURN valid;
END //
DELIMITER ;

-- **********************************************

INSERT INTO staff
	(username, password, first_name, last_name)
VALUES
('admin', '$2a$11$5jbYgpHIbvseZzGa/x9NMOSdbCKQrkjYrtkioz4GvZPvmdfXqQoDK', '', ''),
('emil','$2a$11$TYqlQo/VdYQs6bf0L8e4we4TM0C65.4IiYb0bHUEqprsvJllzqEAy', 'Emilio', 'Jacinto'),
('johniga','$2a$11$UVovi.14u7YZRMHADjSBVeCN5Bma1Ny22zf95mXJVL/8lu9lhAQt2', 'Jonathan', 'Negrillo');

INSERT INTO ticket_types
	(type_name, price)
VALUES
	('Single', 15),
    
	('School Field Trip', 15),
	('Family Bundle ', 15),
	('Group Rate', 15),
	('Art Exhibit', 15);

INSERT INTO events
	(event_name, start_date, end_date)
VALUES
	('World War II Exhibition', '2026-03-31', '2026-03-31'),
	('Dinosaur Fossil Exhibition', '2026-04-06', '2026-04-06'),
	('Classical Art Gallery', '2026-04-08', '2026-04-08'),
	('Ethnic Basket Weaving Workshop', '2026-04-08', '2026-04-10'),
	('Techno City Exposition', '2026-04-08', '2026-04-10'),
	('Continuing Legal Education', '2026-04-06', '2026-04-07');
    
    
	//('World War II Exhibition', '2026-04-04', '2026-04-06'),