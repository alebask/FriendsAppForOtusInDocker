CREATE TABLE `account` (
  `accountid` binary(16) NOT NULL DEFAULT (uuid_to_bin(uuid())),
  `profileid` int DEFAULT NULL,
  `email` varchar(256) NOT NULL,
  `passwordhash` longtext NOT NULL,
  `changedon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`accountid`),
  UNIQUE KEY `email_UNIQUE` (`email`),
  UNIQUE KEY `accountid_UNIQUE` (`accountid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `friendship` (
  `requestedby` int NOT NULL,
  `requestedto` int NOT NULL,
  `status` int NOT NULL,
  `changedon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `'requestedtby_requestedto_unique` (`requestedby`,`requestedto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `page` (
  `pageid` int NOT NULL AUTO_INCREMENT,
  `profileid` int DEFAULT NULL,
  `changedon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `url` varchar(500) NOT NULL,
  `title` varchar(200) NOT NULL,
  `content` text NOT NULL,
  PRIMARY KEY (`pageid`),
  UNIQUE KEY `pageid_UNIQUE` (`pageid`),
  UNIQUE KEY `profileid_url_unique` (`profileid`,`url`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile` (
  `profileid` int NOT NULL AUTO_INCREMENT,
  `changedon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `firstname` varchar(256) NOT NULL,
  `lastname` varchar(256) NOT NULL,
  `age` int NOT NULL,
  `gender` varchar(256) NOT NULL,
  `city` varchar(256) NOT NULL,
  `interests` text NOT NULL,
  PRIMARY KEY (`profileid`),
  UNIQUE KEY `profileid_UNIQUE` (`profileid`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
