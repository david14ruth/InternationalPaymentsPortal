
<img width="1363" height="707" alt="merr" src="https://github.com/user-attachments/assets/b4e4fb61-84e5-4c3f-95f4-0c595d1baa66" />

Youtube front end  demo : https://www.youtube.com/watch?v=3KaLPcppwgc
APPLICATION-DEVELOPMENT-SECURITY-Project APDS International Payments Portal Project Overview

This repository contains the development and security implementation of an internal International Payments Portal for a bank. The project consists of two main portals:

Customer Portal – allows registered customers to securely log in, enter international payment details, and submit payments. Employee Portal – allows bank employees to securely log in, review customer payments, verify SWIFT codes, and submit transactions to SWIFT.

Security is a core focus in this project, with measures in place to prevent common web vulnerabilities and protect sensitive banking data.

This is a group project developed collaboratively by our internal development team.

Team Members Name Role / Contribution

Ishka Sewshanker Frontend – HTML, CSS, JavaScript (Customer Portal UI) Xolisile Princess Mnyandu Frontend – HTML, CSS, JavaScript (Employee Portal UI) Franklin ngangu 3 Backend – ASP.NET Core API, Database Integration David oyowa 4 Security Implementation – Input Validation, Password Hashing, SSL Configuration Chantel Mafunise Testing & Tools – MobSF, ScoutSuite, SonarQube, CI/CD with CircleCI Mororiseng Jesicca Legodi Documentation & Video Demonstration, README, Submission Prep

Features Customer Portal Secure user registration with full name, ID number, account number, and password. Passwords stored using hashing and salting (ASP.NET Core Identity). Input validation using RegEx whitelisting. Encrypted traffic over SSL/HTTPS. Protection against: Session hijacking Clickjacking SQL Injection Cross-Site Scripting (XSS) Man-in-the-Middle attacks DDoS attacks Employee Portal Pre-registered employees only (no registration process). Secure login with password hashing and salting. Input validation using RegEx whitelisting. Review, verify, and submit customer transactions to SWIFT. Same security measures as the Customer Portal. Tech Stack Frontend: HTML, CSS, JavaScript Backend/API: ASP.NET Core Database: SQL Server (MSSQL) Security Tools: MobSF, ScoutSuite, SonarQube, CircleCI Deployment: SSL-enabled server, tested against AWS (ScoutSuite) Security Tools MobSF – Static and dynamic mobile application security analysis. ScoutSuite – Cloud infrastructure security assessment for AWS. SonarQube – Code quality and vulnerability detection. CircleCI – CI/CD pipeline to automate builds, tests, and scans.
