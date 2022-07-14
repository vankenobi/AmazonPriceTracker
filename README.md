


![Logo](https://drive.google.com/uc?export=view&id=1vpSXOnTEeH1hW_4rWLQiZHVeul01W6ee)

<h3 align="left">Languages and Tools:</h3>
<p align="left"> <a href="https://www.w3schools.com/cs/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/csharp/csharp-original.svg" alt="csharp" width="40" height="40"/> </a> <a href="https://www.docker.com/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/docker/docker-original-wordmark.svg" alt="docker" width="40" height="40"/> </a> <a href="https://www.postgresql.org" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/postgresql/postgresql-original-wordmark.svg" alt="postgresql" width="40" height="40"/> </a> </p>

# Amazon Price Tracker API

Amazon price tracker is an open source project that followed the prices. Perfect to run on a docker container. This API includes some methods for price tracking. 



#  Used Technologies And Architecture

![Onion Architecture](https://drive.google.com/uc?export=view&id=1ROCLMRuQ8lhgoFui_cjdCPJNVWbpTmsy)

#### **What is Onion Architecture ?** 

Most of the traditional architectures raise fundamental issues of tight coupling and separation of concerns. Onion Architecture was introduced by Jeffrey Palermo to provide a better way to build applications in perspective of better testability, maintainability, and dependability. Onion Architecture addresses the challenges faced with 3-tier and n-tier architectures, and to provide a solution for common problems. Onion architecture layers interact to each other by using the Interfaces. C# programmers are drawn to Onion Architecture due to the dependency flows.

#### **Principles**

Onion Architecture is based on the inversion of control principle. Onion Architecture is comprised of multiple concentric layers interfacing each other towards the core that represents the domain. The architecture does not depend on the data layer as in classic multi-tier architectures, but on the actual domain models.

#### **Repository Pattern**

![Repository Pattern](https://drive.google.com/uc?export=view&id=11fzxq8Ye5v3Un0Y7hzpW-xZFHlx3iUa0)

The Repository design pattern is a data access pattern used in software development.
Repository design pattern as defined by Martin Fowler isolates your domain from caring about how storage is implemented so all objects retrieved can be treated like an in-memory collection.
You could have a repository based on a database, an XML file, a text document, a web service, or anything.
The applications code itself doesn’t care. This makes it very useful for testing.

**What are Repositories?**

* Repositories are classes or components that encapsulate the logic required to access persistence store.

* Repositories, in practice, are used to perform database operations for domain objects (Entity and Value types).

* Generally, a separate repository is used for each Entity (or Aggregate Root).

**Purpose of Repositories**
* Centralize common data access functionality.
* Provide better maintainability.
* Decouple the infrastructure or technology used to access persistence store from the domain model layer.
* Make your code testable, reusable, and maintainable.

#### **Database: Postgresql**

<img src="https://drive.google.com/uc?export=view&id=1u1n1-Nw6eQw9AYOXJY6GN1ZhBI2wwIaR" alt="MarineGEO circle logo" style="width:50%;"/>

PostgreSQL evolved from the Ingres project at the University of California, Berkeley. In 1982, the leader of the Ingres team, Michael Stonebraker, left Berkeley to make a proprietary version of Ingres. He returned to Berkeley in 1985, and began a post-Ingres project to address the problems with contemporary database systems that had become increasingly clear during the early 1980s. He won the Turing Award in 2014 for these and other projects, and techniques pioneered in them.
The new project, POSTGRES, aimed to add the fewest features needed to completely support data types. These features included the ability to define types and to fully describe relationships – something used widely, but maintained entirely by the user. In POSTGRES, the database understood relationships, and could retrieve information in related tables in a natural way using rules. POSTGRES used many of the ideas of Ingres, but not its code.





## ScreenShot 

![Logo](https://drive.google.com/uc?export=view&id=1vzDqcmSXk55V3AOQnBLbrUWNyRL7ixZ2)



## Source

* https://www.codeguru.com/csharp/understanding-onion-architecture/

* https://psid23.medium.com/repository-pattern-for-data-access-in-software-development-4c10aa9604da

* https://en.wikipedia.org/wiki/PostgreSQL
