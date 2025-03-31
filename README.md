# Unturned Play To Earn
Base template for running a server with play to earn support

## Functionality
- Killing zombies will earn PTE
- Playing will earn PTE
- Killing players will earn PTE
- Collecting specific item will earn PTE
- Setup wallet as command /wallet 0x123...

## Using
Download [Unturned](https://unturned.fandom.com/wiki/Hosting_a_Dedicated_Server) server files
- Install a database like mysql or mariadb
- Create a user for the database: GRANT ALL PRIVILEGES ON pte_wallets.* TO 'pte_admin'@'localhost' IDENTIFIED BY 'supersecretpassword' WITH GRANT OPTION; FLUSH PRIVILEGES;
- Create a table named unturned:
```sql
CREATE TABLE unturned (
    uniqueid VARCHAR(255) NOT NULL PRIMARY KEY,
    walletaddress VARCHAR(255) DEFAULT null,
    value DECIMAL(50, 0) NOT NULL DEFAULT 0
);
```
- Download [latest release](https://github.com/Play-To-Earn-Currency/unturned/releases)
- Install [rocket](https://www.youtube.com/watch?v=A1Dt2__FhvQ) on your dedicated server
- Put the [latest release](https://github.com/Play-To-Earn-Currency/unturned/releases) inside rocket folder from your dedicated server
- Start the server once
- Configure PlayToEarn inside Rocket/Plugins/ with your database credentials and earning quantities
- Start the server normally