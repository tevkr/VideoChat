CREATE TABLE IF NOT EXISTS users (
  	id TEXT NOT NULL,
	username TEXT NOT NULL,
	password TEXT NOT NULL,
	PRIMARY KEY (id, username)
);