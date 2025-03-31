# Unturned Play To Earn
Base template for running a server with play to earn support

## Functionality
- Killing zombies will earn PTE
- Playing will earn PTE
- Setup wallet as command /wallet 0x123...

## Using
Download [Unturned](https://unturned.fandom.com/wiki/Hosting_a_Dedicated_Server) server files
- Install a database like mysql or mariadb
- Create a user for the database: GRANT ALL PRIVILEGES ON pte_wallets.* TO 'pte_admin'@'localhost' IDENTIFIED BY 'supersecretpassword' WITH GRANT OPTION; FLUSH PRIVILEGES;
- Create a table named unturned:
```sql
CREATE TABLE nmrih (
    uniqueid VARCHAR(255) NOT NULL PRIMARY KEY,
    walletaddress VARCHAR(255) DEFAULT null,
    value DECIMAL(50, 0) NOT NULL DEFAULT 0
);
```
- Download latest release
- Install [rocket](https://steamcommunity.com/sharedfiles/filedetails/?id=1809109826) on your dedicated server