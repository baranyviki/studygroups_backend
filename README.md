# studygroups_backend
# Deployment:

#Setup graph database
1.st get Neo4j Graph database Community edition
https://neo4j.com/download-center/#community
Version: 3.5.13

unzip 
copy to folder > that wil be %Neo4j folder
get apoc 
https://github.com/neo4j-contrib/neo4j-apoc-procedures/releases/tag/3.5.0.5
Copy apoc jar to 
%Neo4j/Plugins


dbms.security.procedures.unrestricted=apoc.*
apoc.import.file.enabled=true
apoc.import.file.use_neo4j_config=true
apoc.uuid.enabled=true


uncomment http and bolt
++
dbms.security.procedures.unrestricted=apoc.*
apoc.import.file.enabled=true
apoc.import.file.use_neo4j_config=true
apoc.uuid.enabled=true

CONF MOD: neo4j stop
update-service
neo4j start




https://neo4j.com/download-center/#desktop
Version: 1.2.3


