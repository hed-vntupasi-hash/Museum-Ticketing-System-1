USE museum_ticketing_system;

DELIMITER //
CREATE FUNCTION
	GetTicketId(qr VARCHAR(50))
RETURNS INT
DETERMINISTIC
BEGIN
	DECLARE id INT;
    
	SELECT ticket_id
    INTO id
	FROM tickets
	WHERE qrcode = qr;
    
    RETURN id;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE
	GetTicketValidity(qrcode VARCHAR(50))
BEGIN
    DECLARE now DATETIME;
    DECLARE start DATETIME;
    DECLARE end DATETIME;
    
    DECLARE isValid BOOLEAN;
    DECLARE message VARCHAR(255);
    
    SET now = NOW();
    
	SELECT
		CONCAT(E.start_date, ' 00:00:00'),
		CONCAT(E.end_date, ' 23:59:59')
	INTO start, end
	FROM tickets AS T
    LEFT JOIN events AS E
    ON T.event_id = E.event_id
    WHERE T.ticket_id = GetTicketId(qrcode)
    LIMIT 1;
    
    SET isValid = FALSE;
	IF now < start THEN
		SET message = CONCAT('Ticket Not Valid Before ', DATE(start));
	ELSEIF now > end THEN
		SET message = CONCAT('Ticket Expired After ', DATE(end));
	ELSEIF now > start AND now < end THEN
		SET message = CONCAT('Proceed.');
		SET isValid = TRUE;
	ELSE 
		SET message = CONCAT('Invalid QR code.');
	END IF;
    
    SELECT isValid, message;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE
	PurchaseTicket(IN ticket_type_id INT, IN event_id INT, IN qr VARCHAR(50))
BEGIN
	DECLARE id int;
	INSERT INTO tickets (ticket_type_id, event_id, qrcode)
    VALUES (ticket_type_id, event_id, qr);
END //
DELIMITER ;


-- CALL PurchaseTicket(2, 2, 'gggg');

-- CALL GetTicketValidity('085OBTjq9iC3');