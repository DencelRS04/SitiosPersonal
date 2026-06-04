
CREATE DATABASE  IF NOT EXISTS `db_personal_sitios` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci */;
USE `db_personal_sitios`;
-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: 138.59.135.33    Database: db_personal_sitios
-- ------------------------------------------------------
-- Server version	5.5.5-10.5.29-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `accion_personal`
--

DROP TABLE IF EXISTS `accion_personal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accion_personal` (
  `id_accion` int(11) NOT NULL AUTO_INCREMENT,
  `tipo_accion` enum('CONTRATACION','ASCENSO','TRASLADO','DESPIDO','OTRO') NOT NULL,
  `fecha_accion` date NOT NULL,
  `descripcion` varchar(500) NOT NULL,
  `id_empleado` int(11) NOT NULL,
  `id_aprobador` int(11) NOT NULL,
  PRIMARY KEY (`id_accion`),
  KEY `id_empleado` (`id_empleado`),
  KEY `id_aprobador` (`id_aprobador`),
  CONSTRAINT `accion_personal_ibfk_1` FOREIGN KEY (`id_empleado`) REFERENCES `empleado` (`id_empleado`),
  CONSTRAINT `accion_personal_ibfk_2` FOREIGN KEY (`id_aprobador`) REFERENCES `empleado` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accion_personal`
--

LOCK TABLES `accion_personal` WRITE;
/*!40000 ALTER TABLE `accion_personal` DISABLE KEYS */;
INSERT INTO `accion_personal` VALUES (1,'CONTRATACION','2019-01-15','Contratación como Jefe de Recursos Humanos',1,1),(2,'CONTRATACION','2020-08-01','Contratación como Médico General en área clínica',2,1),(3,'CONTRATACION','2022-03-01','Contratación como Enfermera Especializada UCI',3,1),(4,'CONTRATACION','2018-06-15','Contratación como Desarrollador de Software',4,1),(5,'CONTRATACION','2010-07-01','Contratación como Enfermera Especializada emergencias',5,1),(6,'CONTRATACION','2016-04-01','Contratación como Médico Especialista en cardiología',6,1),(7,'CONTRATACION','2016-02-01','Contratación como Inspector de Seguridad Ocupacional',7,1),(8,'CONTRATACION','2012-03-01','Contratación como Técnico en Radiología',8,1),(9,'ASCENSO','2021-06-01','Ascenso a Jefe del Área de Recursos Humanos',1,1),(10,'ASCENSO','2023-01-15','Ascenso a Médico Especialista en Medicina Interna',2,1),(11,'TRASLADO','2022-09-01','Traslado al turno nocturno en emergencias',5,1),(12,'OTRO','2023-11-01','Reconocimiento por desempeño sobresaliente',6,1);
/*!40000 ALTER TABLE `accion_personal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `area`
--

DROP TABLE IF EXISTS `area`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `area` (
  `id_area` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `id_jefatura` int(11) DEFAULT NULL,
  PRIMARY KEY (`id_area`),
  UNIQUE KEY `codigo` (`codigo`),
  KEY `fk_area_jefatura` (`id_jefatura`),
  CONSTRAINT `fk_area_jefatura` FOREIGN KEY (`id_jefatura`) REFERENCES `empleado` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `area`
--

LOCK TABLES `area` WRITE;
/*!40000 ALTER TABLE `area` DISABLE KEYS */;
INSERT INTO `area` VALUES (1,'ARE-001','Dirección General',NULL),(2,'ARE-002','Recursos Humanos',1),(3,'ARE-003','Medicina General',2),(4,'ARE-004','Enfermería',3),(5,'ARE-005','Tecnologías de Información',4),(6,'ARE-006','Administración y Finanzas',NULL),(7,'ARE-007','Radiología e Imágenes',8),(8,'ARE-008','Farmacia',NULL),(9,'ARE-009','Seguridad Ocupacional',7);
/*!40000 ALTER TABLE `area` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bitacora`
--

DROP TABLE IF EXISTS `bitacora`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bitacora` (
  `id_bitacora` int(11) NOT NULL AUTO_INCREMENT,
  `fecha` datetime DEFAULT current_timestamp(),
  `id_usuario` int(11) NOT NULL,
  `tipo` enum('INSERT','UPDATE','DELETE','SELECT','ERROR') NOT NULL,
  `entidad` varchar(100) DEFAULT NULL,
  `datos_anteriores` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`datos_anteriores`)),
  `datos_nuevos` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`datos_nuevos`)),
  `descripcion` longtext DEFAULT NULL,
  PRIMARY KEY (`id_bitacora`),
  KEY `idx_bitacora_fecha` (`fecha`),
  KEY `idx_bitacora_usuario` (`id_usuario`),
  KEY `idx_bitacora_tipo` (`tipo`),
  CONSTRAINT `bitacora_ibfk_1` FOREIGN KEY (`id_usuario`) REFERENCES `usuario` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=364 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bitacora`
--

LOCK TABLES `bitacora` WRITE;
/*!40000 ALTER TABLE `bitacora` DISABLE KEYS */;
INSERT INTO `bitacora` VALUES (1,'2026-05-28 09:32:09',1,'INSERT','rol',NULL,'{\"id_rol\":1,\"nombre\":\"Administrador\",\"activo\":true}','Inserción de nuevo rol: Administrador'),(2,'2026-05-28 09:32:09',1,'INSERT','usuario',NULL,'{\"id_usuario\":2,\"usuario\":\"mfernandez\",\"nombre_completo\":\"María Fernández Rojas\",\"estado\":\"ACTIVO\"}','Inserción de nuevo usuario: mfernandez'),(3,'2026-05-28 09:32:09',1,'INSERT','concurso',NULL,'{\"id_concurso\":5,\"codigo\":\"CONC-2026-001\",\"nombre\":\"Reclutamiento Médicos Especialistas Q1 2026\",\"estado\":\"VIGENTE\"}','Inserción de nuevo concurso: CONC-2026-001'),(4,'2026-05-28 09:32:09',2,'INSERT','oferente',NULL,'{\"id_oferente\":1,\"identificacion\":\"101230456\",\"nombre_completo\":\"Ana Lucía Ramírez Pérez\",\"tipo_identificacion\":\"CEDULA\"}','Inserción de nuevo oferente: Ana Lucía Ramírez Pérez'),(5,'2026-05-28 09:32:09',2,'INSERT','entrevista',NULL,'{\"id_entrevista\":1,\"id_oferente\":1,\"fecha_entrevista\":\"2020-07-15 09:00:00\",\"estado\":\"PENDIENTE\"}','Inserción de nueva entrevista para oferente ID 1'),(6,'2026-05-28 09:32:09',2,'UPDATE','entrevista','{\"id_entrevista\":1,\"estado\":\"PENDIENTE\",\"observacion\":null}','{\"id_entrevista\":1,\"estado\":\"REALIZADA\",\"observacion\":\"Excelente perfil médico, muy buena experiencia clínica.\"}','Actualización de entrevista ID 1: marcada como REALIZADA'),(7,'2026-05-28 09:32:09',1,'INSERT','empleado',NULL,'{\"id_empleado\":1,\"numero_empleado\":\"EMP-0001\",\"id_oferente\":4,\"id_puesto\":2,\"fecha_ingreso\":\"2019-01-15\"}','Inserción de nuevo empleado: EMP-0001 - Diego Vargas'),(8,'2026-05-28 09:32:09',1,'INSERT','accion_personal',NULL,'{\"id_accion\":1,\"tipo_accion\":\"CONTRATACION\",\"fecha_accion\":\"2019-01-15\",\"id_empleado\":1}','Acción de personal CONTRATACION generada para empleado EMP-0001'),(9,'2026-05-28 09:32:09',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(10,'2026-05-28 09:32:09',1,'SELECT','empleado',NULL,NULL,'El usuario consulta empleado'),(11,'2026-05-28 09:32:09',2,'SELECT','oferente',NULL,NULL,'El usuario consulta oferente'),(12,'2026-05-28 09:32:09',1,'UPDATE','usuario','{\"id_usuario\":6,\"estado\":\"ACTIVO\",\"intentos_login\":3}','{\"id_usuario\":6,\"estado\":\"BLOQUEADO\",\"intentos_login\":3,\"fecha_bloqueo\":\"2026-05-20 08:35:00\"}','Usuario rsanchez bloqueado por 3 intentos fallidos de login'),(13,'2026-05-28 09:32:09',1,'ERROR','usuario',NULL,NULL,'Error al intentar eliminar usuario con roles asignados: No se puede eliminar un registro con datos relacionados.'),(14,'2026-05-28 11:59:38',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(15,'2026-05-28 12:00:49',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(16,'2026-05-28 12:01:01',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(17,'2026-05-28 12:01:14',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(18,'2026-05-28 12:03:51',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(19,'2026-05-28 12:11:07',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(20,'2026-05-28 12:11:16',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(21,'2026-05-28 12:11:19',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(22,'2026-05-28 12:11:21',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(23,'2026-05-28 12:11:22',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(24,'2026-05-28 12:12:01',1,'INSERT','Rol',NULL,'{\"id_rol\":3,\"nombre\":\"Supervisor\",\"activo\":true,\"pantallas\":[13,8,10,11,3]}','El usuario registra Rol'),(25,'2026-05-28 12:12:01',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(26,'2026-05-28 12:12:04',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(27,'2026-05-28 12:12:15',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(28,'2026-05-28 12:12:23',1,'UPDATE','Rol','{\"rolAnterior\":{\"id_rol\":3,\"nombre\":\"Supervisor\",\"activo\":true},\"pantallas\":[3,8,10,11,13]}','{\"rolActual\":{\"id_rol\":3,\"nombre\":\"Supervisor\",\"activo\":true},\"pantallas\":[12,13,15,8,10,11,3]}','El usuario actualiza Rol'),(29,'2026-05-28 12:12:23',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(30,'2026-05-28 12:14:02',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(31,'2026-05-28 12:23:31',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(32,'2026-05-28 12:23:35',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(33,'2026-05-28 12:23:39',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(34,'2026-05-28 12:23:48',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(35,'2026-05-28 12:25:37',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(36,'2026-05-28 12:32:46',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(37,'2026-05-28 12:32:51',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(38,'2026-05-28 12:33:01',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(39,'2026-05-28 12:34:32',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(40,'2026-05-28 12:34:59',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(41,'2026-05-28 12:52:04',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(42,'2026-05-28 12:52:07',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(43,'2026-05-28 12:52:13',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(44,'2026-05-28 12:52:24',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(45,'2026-05-28 12:54:12',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(46,'2026-05-28 13:04:09',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(47,'2026-05-28 13:04:14',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(48,'2026-05-28 13:04:18',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(49,'2026-05-28 13:04:19',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(50,'2026-05-28 13:04:27',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(51,'2026-05-28 13:04:30',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(52,'2026-05-28 13:04:32',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(53,'2026-05-28 13:04:33',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(54,'2026-05-28 13:04:36',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(55,'2026-05-28 13:04:40',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(56,'2026-05-28 13:04:45',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(57,'2026-05-28 13:04:49',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(58,'2026-05-28 13:05:07',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(59,'2026-05-28 13:05:10',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(60,'2026-05-28 13:05:16',1,'SELECT','bitacora',NULL,NULL,'El usuario consulta bitacora'),(61,'2026-05-28 13:05:17',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(62,'2026-05-28 13:05:18',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(63,'2026-05-28 13:05:22',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(64,'2026-05-28 13:08:57',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(65,'2026-05-28 13:09:03',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(66,'2026-05-28 13:09:08',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(67,'2026-05-28 13:09:41',1,'INSERT','Rol',NULL,'{\"id_rol\":4,\"nombre\":\"Jefatura\",\"activo\":true,\"pantallas\":[15,4,3]}','El usuario registra Rol'),(68,'2026-05-28 13:10:09',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(69,'2026-05-28 13:10:14',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(70,'2026-05-28 13:10:52',1,'INSERT','Rol',NULL,'{\"id_rol\":6,\"nombre\":\"Profesor\",\"activo\":true,\"pantallas\":[8,10,11]}','El usuario registra Rol'),(71,'2026-05-28 13:10:52',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(72,'2026-05-28 13:11:34',1,'INSERT','Rol',NULL,'{\"id_rol\":7,\"nombre\":\"Recursos Humanos\",\"activo\":true,\"pantallas\":[12,13,14,15,4,9,1,3]}','El usuario registra Rol'),(73,'2026-05-28 13:11:34',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(74,'2026-05-28 13:11:57',1,'INSERT','Rol',NULL,'{\"id_rol\":8,\"nombre\":\"Encargado\",\"activo\":true,\"pantallas\":[11,3]}','El usuario registra Rol'),(75,'2026-05-28 13:11:58',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(76,'2026-05-28 13:12:04',1,'INSERT','Rol',NULL,'{\"id_rol\":9,\"nombre\":\"AAA\",\"activo\":true,\"pantallas\":[]}','El usuario registra Rol'),(77,'2026-05-28 13:12:04',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(78,'2026-05-28 13:12:08',1,'DELETE','Rol','{\"id_rol\":9,\"nombre\":\"AAA\",\"activo\":true}',NULL,'El usuario elimina Rol'),(79,'2026-05-28 13:12:08',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(80,'2026-05-28 13:12:27',1,'INSERT','Rol',NULL,'{\"id_rol\":10,\"nombre\":\"Sin asociaci\\u00F3n\",\"activo\":true,\"pantallas\":[]}','El usuario registra Rol'),(81,'2026-05-28 13:12:28',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(82,'2026-05-28 13:12:50',1,'INSERT','Rol',NULL,'{\"id_rol\":11,\"nombre\":\"Estudiante\",\"activo\":true,\"pantallas\":[10,11]}','El usuario registra Rol'),(83,'2026-05-28 13:12:50',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(84,'2026-05-28 13:13:26',1,'INSERT','Rol',NULL,'{\"id_rol\":12,\"nombre\":\"Limpieza\",\"activo\":true,\"pantallas\":[14]}','El usuario registra Rol'),(85,'2026-05-28 13:13:26',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(86,'2026-05-28 13:13:45',1,'INSERT','Rol',NULL,'{\"id_rol\":13,\"nombre\":\"Prueba con asociacion\",\"activo\":true,\"pantallas\":[13,4]}','El usuario registra Rol'),(87,'2026-05-28 13:13:45',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(88,'2026-05-28 13:13:47',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(89,'2026-05-28 13:13:49',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(90,'2026-05-28 13:14:07',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(91,'2026-05-28 13:14:08',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(92,'2026-05-28 13:14:11',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(93,'2026-05-28 13:14:13',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(94,'2026-05-28 13:14:15',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(95,'2026-05-28 13:14:16',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(96,'2026-05-28 13:14:29',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(97,'2026-05-28 13:14:31',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(98,'2026-05-28 13:14:38',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(99,'2026-05-28 13:14:46',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(100,'2026-05-28 13:15:14',1,'INSERT','Pantalla',NULL,'{\"id_pantalla\":16,\"nombre\":\"Prueba nueva pantalla\",\"roles\":[11]}','El usuario registra Pantalla'),(101,'2026-05-28 13:15:14',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(102,'2026-05-28 13:15:19',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(103,'2026-05-28 13:15:21',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(104,'2026-05-28 13:15:28',1,'UPDATE','Pantalla','{\"pantallaAnterior\":{\"id_pantalla\":16,\"nombre\":\"Prueba nueva pantalla\",\"modulo\":\"Seguridad\",\"ruta\":\"#\",\"icono\":\"fa-window-maximize\",\"orden_menu\":99,\"visible_menu\":true,\"activo\":true},\"roles\":[11]}','{\"pantallaActual\":{\"id_pantalla\":16,\"nombre\":\"Prueba nueva pantalla\",\"modulo\":null,\"ruta\":null,\"icono\":null,\"orden_menu\":0,\"visible_menu\":false,\"activo\":false},\"roles\":[]}','El usuario actualiza Pantalla'),(105,'2026-05-28 13:15:28',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(106,'2026-05-28 13:15:31',1,'DELETE','Pantalla','{\"id_pantalla\":16,\"nombre\":\"Prueba nueva pantalla\",\"modulo\":\"Seguridad\",\"ruta\":\"#\",\"icono\":\"fa-window-maximize\",\"orden_menu\":99,\"visible_menu\":true,\"activo\":true}',NULL,'El usuario elimina Pantalla'),(107,'2026-05-28 13:15:31',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(108,'2026-05-28 13:15:32',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(109,'2026-05-28 13:32:07',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(110,'2026-05-28 13:32:11',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(111,'2026-05-28 13:32:12',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(112,'2026-05-28 13:33:09',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(113,'2026-05-28 13:33:17',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(114,'2026-05-28 13:33:26',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(115,'2026-05-28 13:33:43',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(116,'2026-05-28 13:33:44',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(117,'2026-05-28 13:33:45',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(118,'2026-05-28 13:33:46',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(119,'2026-05-28 13:33:47',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(120,'2026-05-28 13:33:48',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(121,'2026-05-28 13:33:48',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(122,'2026-05-28 13:33:50',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(123,'2026-05-28 13:33:51',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(124,'2026-05-28 13:33:51',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(125,'2026-05-28 13:33:52',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(126,'2026-05-28 13:33:52',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(127,'2026-05-28 13:33:53',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(128,'2026-05-28 13:33:54',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(129,'2026-05-28 13:33:56',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(130,'2026-05-28 13:33:56',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(131,'2026-05-28 13:33:57',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(132,'2026-05-28 13:33:59',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(133,'2026-05-28 13:34:00',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(134,'2026-05-28 13:34:00',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(135,'2026-05-28 13:34:01',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(136,'2026-05-28 13:34:04',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(137,'2026-05-28 13:34:05',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(138,'2026-05-28 13:34:06',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(139,'2026-05-28 13:34:07',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(140,'2026-05-28 13:34:08',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(141,'2026-05-28 13:34:10',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(142,'2026-05-28 13:34:20',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(143,'2026-05-28 13:35:13',1,'INSERT','Usuario',NULL,'{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[]}','El usuario registra Usuario'),(144,'2026-05-28 13:35:13',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(145,'2026-05-28 13:35:51',1,'INSERT','Usuario',NULL,'{\"id_usuario\":8,\"usuario\":\"prueba\",\"nombre_completo\":\"prueba\",\"correo\":\"prueba@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[10]}','El usuario registra Usuario'),(146,'2026-05-28 13:35:51',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(147,'2026-05-28 13:36:18',1,'INSERT','Usuario',NULL,'{\"id_usuario\":9,\"usuario\":\"asldkja\",\"nombre_completo\":\"asfsa\",\"correo\":\"aaa@gmial.com\",\"estado\":\"ACTIVO\",\"roles\":[10]}','El usuario registra Usuario'),(148,'2026-05-28 13:36:18',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(149,'2026-05-28 13:36:42',1,'INSERT','Usuario',NULL,'{\"id_usuario\":10,\"usuario\":\"ffffdfs\",\"nombre_completo\":\"asadadsa\",\"correo\":\"asdasd@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[10]}','El usuario registra Usuario'),(150,'2026-05-28 13:36:42',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(151,'2026-05-28 13:37:08',1,'INSERT','Usuario',NULL,'{\"id_usuario\":11,\"usuario\":\"hfgdhhdh\",\"nombre_completo\":\"hdfhdfhdhf\",\"correo\":\"dddd@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[10]}','El usuario registra Usuario'),(152,'2026-05-28 13:37:08',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(153,'2026-05-28 13:37:13',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(154,'2026-05-28 13:37:15',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(155,'2026-05-28 13:37:17',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(156,'2026-05-28 13:37:18',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(157,'2026-05-28 13:37:44',1,'UPDATE','Usuario','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[]}','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[]}','El usuario actualiza Usuario'),(158,'2026-05-28 13:37:44',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(159,'2026-05-28 13:38:10',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(160,'2026-05-28 13:38:22',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(161,'2026-05-28 13:38:23',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(162,'2026-05-28 13:38:27',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(163,'2026-05-28 13:38:34',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(164,'2026-05-28 13:38:35',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(165,'2026-05-28 13:38:38',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(166,'2026-05-28 13:38:47',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(167,'2026-05-28 13:39:11',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(168,'2026-05-28 13:39:30',1,'UPDATE','Usuario','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[]}','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','El usuario actualiza Usuario'),(169,'2026-05-28 13:39:30',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(170,'2026-05-28 13:39:57',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(171,'2026-05-28 13:40:09',1,'UPDATE','Usuario','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','El usuario actualiza Usuario'),(172,'2026-05-28 13:40:09',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(173,'2026-05-28 14:00:22',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(174,'2026-05-28 14:00:24',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(175,'2026-05-28 14:00:39',1,'UPDATE','Usuario','{\"id_usuario\":1,\"usuario\":\"admin\",\"nombre_completo\":\"Administrador Sistema\",\"correo\":\"admin@serviciosmedicos.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','{\"id_usuario\":1,\"usuario\":\"admin\",\"nombre_completo\":\"Administrador Sistema\",\"correo\":\"admin@serviciosmedicos.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','El usuario actualiza Usuario'),(176,'2026-05-28 14:00:39',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(177,'2026-05-28 14:13:27',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(178,'2026-05-28 14:13:30',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(179,'2026-05-28 14:13:35',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(180,'2026-05-28 14:13:39',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(181,'2026-05-28 14:13:41',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(182,'2026-05-28 14:13:46',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(183,'2026-05-28 14:13:51',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(184,'2026-05-28 14:13:53',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(185,'2026-05-28 14:13:55',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(186,'2026-05-28 14:13:58',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(187,'2026-05-28 14:14:04',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(188,'2026-05-28 14:14:06',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(189,'2026-05-28 14:14:11',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(190,'2026-05-28 15:19:39',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(191,'2026-05-28 15:19:39',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(192,'2026-05-28 15:19:40',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(193,'2026-05-28 15:19:41',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(194,'2026-05-28 15:19:41',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(195,'2026-05-28 15:20:30',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(196,'2026-05-28 15:20:32',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(197,'2026-05-28 15:20:32',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(198,'2026-05-28 15:20:33',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(199,'2026-05-28 15:35:47',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(200,'2026-05-28 15:35:48',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(201,'2026-05-28 15:35:49',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(202,'2026-05-28 15:37:40',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(203,'2026-05-28 15:37:42',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(204,'2026-05-28 15:37:44',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(205,'2026-05-28 15:37:50',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(206,'2026-05-28 15:37:55',1,'DELETE','Usuario','{\"id_usuario\":9,\"usuario\":\"asldkja\",\"nombre_completo\":\"asfsa\",\"correo\":\"aaa@gmial.com\",\"estado\":\"ACTIVO\"}',NULL,'El usuario elimina Usuario. Datos eliminados: {\"id_usuario\":9,\"usuario\":\"asldkja\",\"nombre_completo\":\"asfsa\",\"correo\":\"aaa@gmial.com\",\"estado\":\"ACTIVO\"}'),(207,'2026-05-28 15:37:55',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(208,'2026-05-28 15:37:59',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(209,'2026-05-28 15:38:36',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(210,'2026-05-28 15:40:20',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(211,'2026-05-28 15:40:34',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(212,'2026-05-28 15:41:38',1,'INSERT','Usuario',NULL,'{\"id_usuario\":12,\"usuario\":\"Juan Perez\",\"nombre_completo\":\"Juan P\\u00E9rez Villalta\",\"correo\":\"juanperez@gmial.com\",\"estado\":\"ACTIVO\",\"roles\":[2]}','El usuario registra Usuario'),(213,'2026-05-28 15:41:38',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(214,'2026-05-28 15:42:34',12,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(215,'2026-05-28 15:42:40',12,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(216,'2026-05-28 15:52:49',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(217,'2026-05-28 15:52:59',1,'UPDATE','Usuario','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','{\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','El usuario actualiza Usuario. Datos anteriores: {\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}. Datos nuevos: {\"id_usuario\":7,\"usuario\":\"Dencel\",\"nombre_completo\":\"Dencel Rodr\\u00EDguez Solano\",\"correo\":\"dencel@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}'),(218,'2026-05-28 15:52:59',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(219,'2026-05-28 15:53:17',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(220,'2026-05-28 15:53:18',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(221,'2026-05-28 15:53:19',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(222,'2026-05-28 15:53:21',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(223,'2026-05-28 15:53:22',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(224,'2026-05-28 15:53:23',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(225,'2026-05-28 15:53:24',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(226,'2026-05-28 15:53:25',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(227,'2026-05-29 12:32:17',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(228,'2026-05-29 12:32:21',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(229,'2026-05-29 12:32:22',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(230,'2026-05-29 12:32:23',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(231,'2026-05-29 12:33:43',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(232,'2026-05-29 12:33:49',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(233,'2026-05-29 12:33:51',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(234,'2026-05-29 12:33:54',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(235,'2026-05-29 12:33:55',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(236,'2026-05-29 12:34:03',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(237,'2026-05-29 12:34:06',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(238,'2026-05-29 12:34:12',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(239,'2026-05-29 12:34:16',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(240,'2026-05-29 12:34:21',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(241,'2026-05-29 12:34:27',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(242,'2026-05-29 12:34:31',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(243,'2026-05-29 12:34:34',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(244,'2026-05-29 12:34:36',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(245,'2026-05-29 12:34:37',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(246,'2026-05-29 12:34:38',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(247,'2026-05-29 12:34:38',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(248,'2026-05-31 14:38:29',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(249,'2026-05-31 14:38:31',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(250,'2026-05-31 14:38:32',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(251,'2026-05-31 14:38:33',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(252,'2026-05-31 14:40:20',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(253,'2026-05-31 14:40:21',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(254,'2026-05-31 14:40:22',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(255,'2026-05-31 14:40:22',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(256,'2026-05-31 14:40:36',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(257,'2026-05-31 14:40:38',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(258,'2026-05-31 14:41:04',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(259,'2026-05-31 14:41:10',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(260,'2026-05-31 14:41:12',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(261,'2026-05-31 14:42:03',1,'INSERT','Usuario',NULL,'{\"id_usuario\":13,\"usuario\":\"Andrew\",\"nombre_completo\":\"Andrew Rivera Gamboa\",\"correo\":\"andrew@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}','El usuario registra Usuario. Datos nuevos: {\"id_usuario\":13,\"usuario\":\"Andrew\",\"nombre_completo\":\"Andrew Rivera Gamboa\",\"correo\":\"andrew@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[1]}'),(262,'2026-05-31 14:42:03',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(263,'2026-05-31 14:42:21',13,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(264,'2026-05-31 14:42:23',13,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(265,'2026-05-31 14:42:24',13,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(266,'2026-05-31 14:42:25',13,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(267,'2026-05-31 14:42:31',13,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(268,'2026-05-31 14:42:40',13,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(269,'2026-05-31 14:42:49',13,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(270,'2026-05-31 14:43:29',13,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(271,'2026-05-31 14:43:31',13,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(272,'2026-05-31 14:43:59',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(273,'2026-05-31 14:44:04',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(274,'2026-05-31 14:44:04',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(275,'2026-05-31 14:44:12',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(276,'2026-05-31 14:44:16',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(277,'2026-06-01 12:06:07',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(278,'2026-06-01 12:06:08',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(279,'2026-06-01 12:06:09',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(280,'2026-06-01 12:32:34',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(281,'2026-06-01 12:32:35',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(282,'2026-06-01 12:32:49',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(283,'2026-06-01 12:32:52',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(284,'2026-06-01 13:17:24',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(285,'2026-06-01 13:17:33',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(286,'2026-06-01 13:17:34',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(287,'2026-06-01 13:17:35',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(288,'2026-06-01 13:17:37',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(289,'2026-06-01 13:39:04',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(290,'2026-06-01 13:39:35',1,'INSERT','Compañía',NULL,'{\"id_compania\":2,\"codigo\":\"COMP022\",\"nombre\":\"Prueba\"}','El usuario registra Compañía'),(291,'2026-06-01 13:39:36',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(292,'2026-06-01 13:41:29',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(293,'2026-06-01 13:41:41',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(294,'2026-06-01 13:41:48',1,'UPDATE','Compañía','{\"id_compania\":2,\"codigo\":\"COMP022\",\"nombre\":\"Prueba\"}','{\"id_compania\":2,\"codigo\":\"COMP022\",\"nombre\":\"Prueba 1\"}','El usuario actualiza Compañía'),(295,'2026-06-01 13:41:48',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(296,'2026-06-01 13:42:06',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(297,'2026-06-01 13:42:16',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(298,'2026-06-01 13:42:20',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(299,'2026-06-01 13:42:24',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(300,'2026-06-01 13:42:25',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(301,'2026-06-01 13:42:30',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(302,'2026-06-01 13:42:37',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(303,'2026-06-01 14:12:29',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(304,'2026-06-01 14:12:32',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(305,'2026-06-01 14:12:43',1,'SELECT','Instituciones Educativas',NULL,NULL,'El usuario consulta Instituciones Educativas'),(306,'2026-06-01 14:16:09',1,'INSERT','Ubicación',NULL,'\"Se realiz\\u00F3 la carga de informaci\\u00F3n de provincias, cantones y distritos.\"','El usuario registra Ubicación'),(307,'2026-06-01 14:16:22',1,'INSERT','Ubicación',NULL,'\"Se realiz\\u00F3 la carga de informaci\\u00F3n de provincias, cantones y distritos.\"','El usuario registra Ubicación'),(308,'2026-06-01 14:16:32',1,'INSERT','Ubicación',NULL,'\"Se realiz\\u00F3 la carga de informaci\\u00F3n de provincias, cantones y distritos.\"','El usuario registra Ubicación'),(309,'2026-06-01 14:16:52',1,'INSERT','Ubicación',NULL,'\"Se realiz\\u00F3 la carga de informaci\\u00F3n de provincias, cantones y distritos.\"','El usuario registra Ubicación'),(310,'2026-06-01 14:17:46',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(311,'2026-06-01 18:20:45',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(312,'2026-06-01 18:20:57',1,'INSERT','Ubicación',NULL,'\"Se realiz\\u00F3 la carga de informaci\\u00F3n de provincias, cantones y distritos.\"','El usuario registra Ubicación'),(313,'2026-06-02 12:12:33',1,'SELECT','Parámetros',NULL,NULL,'El usuario consulta Parámetros'),(314,'2026-06-02 12:12:50',1,'SELECT','Compañías',NULL,NULL,'El usuario consulta Compañías'),(315,'2026-06-02 12:12:52',1,'SELECT','Parámetros',NULL,NULL,'El usuario consulta Parámetros'),(316,'2026-06-02 12:48:18',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(317,'2026-06-02 18:09:27',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(318,'2026-06-02 18:09:29',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(319,'2026-06-02 18:09:33',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(320,'2026-06-02 18:09:36',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(321,'2026-06-02 18:09:40',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(322,'2026-06-02 18:09:49',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(323,'2026-06-02 18:09:51',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(324,'2026-06-02 18:12:30',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(325,'2026-06-02 18:12:42',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(326,'2026-06-02 19:27:57',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(327,'2026-06-02 19:28:00',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(328,'2026-06-02 19:28:01',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(329,'2026-06-03 00:21:41',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(330,'2026-06-03 00:21:43',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(331,'2026-06-03 00:21:45',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(332,'2026-06-03 00:21:48',1,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(333,'2026-06-03 00:22:18',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(334,'2026-06-03 00:22:39',1,'INSERT','Rol',NULL,'{\"id_rol\":14,\"nombre\":\"PruebaRazor\",\"pantallas\":[11]}','El usuario registra Rol. Datos nuevos: {\"id_rol\":14,\"nombre\":\"PruebaRazor\",\"pantallas\":[11]}'),(335,'2026-06-03 00:22:39',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(336,'2026-06-03 00:22:43',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(337,'2026-06-03 00:22:53',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(338,'2026-06-03 00:22:56',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(339,'2026-06-03 00:22:58',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(340,'2026-06-03 00:23:10',1,'INSERT','Pantalla',NULL,'{\"id_pantalla\":17,\"nombre\":\"PrebaRazor\",\"roles\":[14]}','El usuario registra Pantalla. Datos nuevos: {\"id_pantalla\":17,\"nombre\":\"PrebaRazor\",\"roles\":[14]}'),(341,'2026-06-03 00:23:11',1,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(342,'2026-06-03 00:23:16',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(343,'2026-06-03 00:24:06',1,'INSERT','Usuario',NULL,'{\"id_usuario\":14,\"usuario\":\"PruebaRazor\",\"nombre_completo\":\"aaa\",\"correo\":\"aaa@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[]}','El usuario registra Usuario. Datos nuevos: {\"id_usuario\":14,\"usuario\":\"PruebaRazor\",\"nombre_completo\":\"aaa\",\"correo\":\"aaa@gmail.com\",\"estado\":\"ACTIVO\",\"roles\":[]}'),(344,'2026-06-03 00:24:07',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(345,'2026-06-03 00:24:17',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(346,'2026-06-03 00:24:53',1,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(347,'2026-06-03 00:25:26',1,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(348,'2026-06-03 00:26:09',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(349,'2026-06-03 00:26:10',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(350,'2026-06-03 00:26:11',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(351,'2026-06-03 00:26:14',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(352,'2026-06-03 00:26:39',7,'UPDATE','Usuario','{\"id_usuario\":12,\"usuario\":\"Juan Perez\",\"nombre_completo\":\"Juan P\\u00E9rez Villalta\",\"correo\":\"juanperez@gmial.com\",\"estado\":\"ACTIVO\",\"roles\":[2]}','{\"id_usuario\":12,\"usuario\":\"Juan Perez\",\"nombre_completo\":\"Juan P\\u00E9rez Villalta\",\"correo\":\"juanperez@gmial.com\",\"estado\":\"ACTIVO\",\"roles\":[2]}','El usuario actualiza Usuario. Datos anteriores: {\"id_usuario\":12,\"usuario\":\"Juan Perez\",\"nombre_completo\":\"Juan P\\u00E9rez Villalta\",\"correo\":\"juanperez@gmial.com\",\"estado\":\"ACTIVO\",\"roles\":[2]}. Datos nuevos: {\"id_usuario\":12,\"usuario\":\"Juan Perez\",\"nombre_completo\":\"Juan P\\u00E9rez Villalta\",\"correo\":\"juanperez@gmial.com\",\"estado\":\"ACTIVO\",\"roles\":[2]}'),(353,'2026-06-03 00:26:39',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(354,'2026-06-03 14:01:38',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(355,'2026-06-03 14:01:47',7,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles'),(356,'2026-06-03 14:01:50',7,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(357,'2026-06-03 14:01:52',7,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(358,'2026-06-03 14:01:55',7,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(359,'2026-06-03 16:01:26',13,'SELECT','Pantallas',NULL,NULL,'El usuario consulta Pantallas'),(360,'2026-06-03 16:01:26',13,'SELECT','Usuarios',NULL,NULL,'El usuario consulta Usuarios'),(361,'2026-06-03 16:01:29',13,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(362,'2026-06-03 16:01:49',13,'SELECT','Bitácora',NULL,NULL,'El usuario consulta Bitácora'),(363,'2026-06-03 16:01:55',13,'SELECT','Roles',NULL,NULL,'El usuario consulta Roles');
/*!40000 ALTER TABLE `bitacora` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `canton`
--

DROP TABLE IF EXISTS `canton`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `canton` (
  `id_canton` int(11) NOT NULL AUTO_INCREMENT,
  `id_provincia` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  PRIMARY KEY (`id_canton`),
  UNIQUE KEY `id_provincia` (`id_provincia`,`nombre`),
  CONSTRAINT `canton_ibfk_1` FOREIGN KEY (`id_provincia`) REFERENCES `provincia` (`id_provincia`)
) ENGINE=InnoDB AUTO_INCREMENT=54 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `canton`
--

LOCK TABLES `canton` WRITE;
/*!40000 ALTER TABLE `canton` DISABLE KEYS */;
INSERT INTO `canton` VALUES (12,1,'Acosta'),(10,1,'Alajuelita'),(6,1,'Aserrí'),(18,1,'Curridabat'),(3,1,'Desamparados'),(17,1,'Dota'),(2,1,'Escazú'),(8,1,'Goicoechea'),(20,1,'León Cortés'),(15,1,'Montes de Oca'),(7,1,'Mora'),(14,1,'Moravia'),(19,1,'Pérez Zeledón'),(4,1,'Puriscal'),(1,1,'San José'),(9,1,'Santa Ana'),(5,1,'Tarrazú'),(13,1,'Tibás'),(16,1,'Turrubares'),(11,1,'Vásquez de Coronado'),(21,2,'Alajuela'),(25,2,'Atenas'),(23,2,'Grecia'),(35,2,'Guatuso'),(34,2,'Los Chiles'),(26,2,'Naranjo'),(29,2,'Orotina'),(27,2,'Palmares'),(28,2,'Poás'),(30,2,'San Carlos'),(24,2,'San Mateo'),(22,2,'San Ramón'),(33,2,'Upala'),(32,2,'Valverde Vega'),(31,2,'Zarcero'),(41,3,'Alvarado'),(36,3,'Cartago'),(43,3,'El Guarco'),(39,3,'Jiménez'),(38,3,'La Unión'),(42,3,'Oreamuno'),(37,3,'Paraíso'),(40,3,'Turrialba'),(45,4,'Barva'),(50,4,'Belén'),(51,4,'Flores'),(44,4,'Heredia'),(49,4,'San Isidro'),(52,4,'San Pablo'),(48,4,'San Rafael'),(47,4,'Santa Bárbara'),(46,4,'Santo Domingo'),(53,4,'Sarapiquí');
/*!40000 ALTER TABLE `canton` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `compania`
--

DROP TABLE IF EXISTS `compania`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compania` (
  `id_compania` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  PRIMARY KEY (`id_compania`),
  UNIQUE KEY `codigo` (`codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `compania`
--

LOCK TABLES `compania` WRITE;
/*!40000 ALTER TABLE `compania` DISABLE KEYS */;
INSERT INTO `compania` VALUES (1,'COMP001','Servicios Medicos SA'),(2,'COMP022','Prueba 1');
/*!40000 ALTER TABLE `compania` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `concurso`
--

DROP TABLE IF EXISTS `concurso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `concurso` (
  `id_concurso` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `estado` enum('VIGENTE','VENCIDO') DEFAULT 'VIGENTE',
  PRIMARY KEY (`id_concurso`),
  UNIQUE KEY `codigo` (`codigo`),
  KEY `idx_concurso_estado` (`estado`),
  KEY `idx_concurso_fechas` (`fecha_inicio`,`fecha_fin`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `concurso`
--

LOCK TABLES `concurso` WRITE;
/*!40000 ALTER TABLE `concurso` DISABLE KEYS */;
INSERT INTO `concurso` VALUES (1,'CONC-2024-001','Reclutamiento Médicos Generales Q1 2024','2024-01-15','2024-02-28','VENCIDO'),(2,'CONC-2024-002','Reclutamiento Enfermeras Especializadas Q2 2024','2024-04-01','2024-05-31','VENCIDO'),(3,'CONC-2025-001','Reclutamiento Personal Administrativo 2025','2025-01-10','2025-03-10','VENCIDO'),(4,'CONC-2025-002','Reclutamiento Técnicos en Radiología 2025','2025-06-01','2025-07-31','VENCIDO'),(5,'CONC-2026-001','Reclutamiento Médicos Especialistas Q1 2026','2026-01-20','2026-06-30','VIGENTE'),(6,'CONC-2026-002','Reclutamiento Enfermeras y Auxiliares 2026','2026-03-01','2026-07-31','VIGENTE'),(7,'CONC-2026-003','Reclutamiento Personal TI y Sistemas 2026','2026-04-15','2026-08-15','VIGENTE');
/*!40000 ALTER TABLE `concurso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `distrito`
--

DROP TABLE IF EXISTS `distrito`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `distrito` (
  `id_distrito` int(11) NOT NULL AUTO_INCREMENT,
  `id_canton` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  PRIMARY KEY (`id_distrito`),
  UNIQUE KEY `id_canton` (`id_canton`,`nombre`),
  CONSTRAINT `distrito_ibfk_1` FOREIGN KEY (`id_canton`) REFERENCES `canton` (`id_canton`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `distrito`
--

LOCK TABLES `distrito` WRITE;
/*!40000 ALTER TABLE `distrito` DISABLE KEYS */;
INSERT INTO `distrito` VALUES (1,1,'Carmen'),(4,1,'Catedral'),(10,1,'Hatillo'),(3,1,'Hospital'),(8,1,'Mata Redonda'),(2,1,'Merced'),(9,1,'Pavas'),(6,1,'San Francisco de Dos Ríos'),(11,1,'San Sebastián'),(7,1,'Uruca'),(5,1,'Zapote'),(12,2,'Escazú'),(13,2,'San Antonio'),(14,2,'San Rafael'),(15,3,'Desamparados'),(20,3,'Frailes'),(19,3,'San Antonio'),(17,3,'San Juan de Dios'),(16,3,'San Miguel'),(18,3,'San Rafael Arriba'),(25,36,'Aguacaliente'),(23,36,'Carmen'),(27,36,'Corralillo'),(29,36,'Dulce Nombre'),(26,36,'Guadalupe'),(30,36,'Llano Grande'),(22,36,'Occidental'),(21,36,'Oriental'),(31,36,'Quebradilla'),(24,36,'San Nicolás'),(28,36,'Tierra Blanca'),(32,44,'Heredia'),(33,44,'Mercedes'),(34,44,'San Francisco'),(35,44,'Ulloa'),(36,44,'Varablanca');
/*!40000 ALTER TABLE `distrito` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleado`
--

DROP TABLE IF EXISTS `empleado`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleado` (
  `id_empleado` int(11) NOT NULL AUTO_INCREMENT,
  `numero_empleado` varchar(50) NOT NULL,
  `id_oferente` int(11) NOT NULL,
  `id_puesto` int(11) NOT NULL,
  `fecha_ingreso` date NOT NULL,
  PRIMARY KEY (`id_empleado`),
  UNIQUE KEY `numero_empleado` (`numero_empleado`),
  UNIQUE KEY `id_oferente` (`id_oferente`),
  KEY `id_puesto` (`id_puesto`),
  KEY `idx_empleado_numero` (`numero_empleado`),
  CONSTRAINT `empleado_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`),
  CONSTRAINT `empleado_ibfk_2` FOREIGN KEY (`id_puesto`) REFERENCES `puesto` (`id_puesto`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleado`
--

LOCK TABLES `empleado` WRITE;
/*!40000 ALTER TABLE `empleado` DISABLE KEYS */;
INSERT INTO `empleado` VALUES (1,'EMP-0001',4,2,'2019-01-15'),(2,'EMP-0002',1,6,'2020-08-01'),(3,'EMP-0003',3,9,'2022-03-01'),(4,'EMP-0004',2,12,'2018-06-15'),(5,'EMP-0005',5,9,'2010-07-01'),(6,'EMP-0006',7,7,'2016-04-01'),(7,'EMP-0007',8,22,'2016-02-01'),(8,'EMP-0008',13,18,'2012-03-01');
/*!40000 ALTER TABLE `empleado` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entrevista`
--

DROP TABLE IF EXISTS `entrevista`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entrevista` (
  `id_entrevista` int(11) NOT NULL AUTO_INCREMENT,
  `id_oferente` int(11) NOT NULL,
  `id_empleado_entrevistador` int(11) NOT NULL,
  `fecha_entrevista` datetime NOT NULL,
  `estado` enum('PENDIENTE','REALIZADA') DEFAULT 'PENDIENTE',
  `observacion` varchar(500) DEFAULT NULL,
  `fecha_creacion` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id_entrevista`),
  KEY `id_oferente` (`id_oferente`),
  KEY `id_empleado_entrevistador` (`id_empleado_entrevistador`),
  KEY `idx_entrevista_fecha` (`fecha_entrevista`),
  KEY `idx_entrevista_estado` (`estado`),
  CONSTRAINT `entrevista_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`),
  CONSTRAINT `entrevista_ibfk_2` FOREIGN KEY (`id_empleado_entrevistador`) REFERENCES `empleado` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entrevista`
--

LOCK TABLES `entrevista` WRITE;
/*!40000 ALTER TABLE `entrevista` DISABLE KEYS */;
INSERT INTO `entrevista` VALUES (1,1,1,'2020-07-15 09:00:00','REALIZADA','Excelente perfil médico, muy buena experiencia clínica. Aprobada para contratación.','2026-05-28 09:32:09'),(2,3,1,'2022-02-10 10:30:00','REALIZADA','Buen manejo de protocolos de enfermería UCI. Se recomienda contratación.','2026-05-28 09:32:09'),(3,2,1,'2018-05-20 11:00:00','REALIZADA','Sólidos conocimientos técnicos en desarrollo .NET. Aprobado.','2026-05-28 09:32:09'),(4,4,1,'2019-01-08 09:30:00','REALIZADA','Gran experiencia en gestión de RRHH. Perfil idóneo para el puesto.','2026-05-28 09:32:09'),(5,7,1,'2016-03-15 14:00:00','REALIZADA','Médico con amplia experiencia en cardiología. Contratado.','2026-05-28 09:32:09'),(6,8,1,'2016-01-20 08:00:00','REALIZADA','Cumple con todos los requisitos de seguridad ocupacional.','2026-05-28 09:32:09'),(7,13,1,'2012-02-10 10:00:00','REALIZADA','Técnico con experiencia en equipos de radiología modernos.','2026-05-28 09:32:09'),(8,5,1,'2026-06-02 09:00:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(9,6,4,'2026-06-03 10:00:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(10,9,1,'2026-06-04 09:30:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(11,10,2,'2026-06-05 11:00:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(12,11,4,'2026-06-06 14:00:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(13,12,1,'2026-06-09 09:00:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(14,14,2,'2026-06-10 10:30:00','PENDIENTE',NULL,'2026-05-28 09:32:09'),(15,15,4,'2026-06-11 08:30:00','PENDIENTE',NULL,'2026-05-28 09:32:09');
/*!40000 ALTER TABLE `entrevista` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `experiencia_laboral`
--

DROP TABLE IF EXISTS `experiencia_laboral`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `experiencia_laboral` (
  `id_experiencia` int(11) NOT NULL AUTO_INCREMENT,
  `id_oferente` int(11) NOT NULL,
  `empresa` varchar(100) NOT NULL,
  `puesto` varchar(100) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  PRIMARY KEY (`id_experiencia`),
  KEY `id_oferente` (`id_oferente`),
  CONSTRAINT `experiencia_laboral_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `experiencia_laboral`
--

LOCK TABLES `experiencia_laboral` WRITE;
/*!40000 ALTER TABLE `experiencia_laboral` DISABLE KEYS */;
INSERT INTO `experiencia_laboral` VALUES (1,1,'Hospital Mexico','Medico General','2015-01-01','2020-12-31'),(2,1,'Clinica Biblica','Medico Especialista','2021-01-01','2023-06-30'),(3,2,'Grupo ICE','Desarrollador de Software','2013-06-01','2017-12-31'),(4,2,'BAC Credomatic','Analista de Sistemas','2018-01-01','2023-12-31'),(5,3,'Hospital San Juan de Dios','Enfermera General','2019-03-01','2022-12-31'),(6,4,'Cooperativa Dos Pinos','Asistente de RRHH','2015-01-01','2018-12-31'),(7,4,'Grupo Monge','Jefe de Recursos Humanos','2019-01-01','2024-12-31'),(8,5,'CCSS Heredia','Enfermera Especialista','2010-06-01','2023-12-31'),(9,6,'Empresa Contable Brenes SA','Contador Junior','2017-01-01','2020-12-31'),(10,7,'Clinica Biblica','Medico General','2016-03-01','2024-01-31'),(11,8,'INS','Tecnico en Seguridad','2009-01-01','2015-12-31'),(12,8,'CCSS','Inspector de Seguridad','2016-01-01','2023-06-30'),(13,9,'Farmacia Fischel','Auxiliar de Farmacia','2020-06-01','2024-12-31'),(14,10,'Cleveland Clinic Miami','Medico Especialista Senior','2014-01-01','2025-01-31'),(15,11,'Avantica Technologies','Desarrollador Full Stack','2016-03-01','2024-12-31'),(16,12,'Consultorio Torres','Psicologa Clinica','2019-01-01','2024-12-31'),(17,13,'Hospital Rafael Calderón','Tecnico en Radiologia','2012-01-01','2023-12-31'),(18,14,'IMAS','Trabajadora Social','2007-01-01','2023-12-31'),(19,15,'CCSS Cartago','Soporte de Sistemas','2019-06-01','2024-12-31');
/*!40000 ALTER TABLE `experiencia_laboral` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `institucion_educativa`
--

DROP TABLE IF EXISTS `institucion_educativa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `institucion_educativa` (
  `id_institucion` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  PRIMARY KEY (`id_institucion`),
  UNIQUE KEY `codigo` (`codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `institucion_educativa`
--

LOCK TABLES `institucion_educativa` WRITE;
/*!40000 ALTER TABLE `institucion_educativa` DISABLE KEYS */;
INSERT INTO `institucion_educativa` VALUES (1,'UCR','Universidad de Costa Rica'),(2,'TEC','Instituto Tecnologico de Costa Rica'),(3,'UNA','Universidad Nacional de Costa Rica'),(4,'UNED','Universidad Estatal a Distancia'),(5,'UCR-MED','Universidad de Costa Rica Sede Medica'),(6,'ULACIT','Universidad Latinoamericana de Ciencia y Tecnologia'),(7,'ULATINA','Universidad Latina de Costa Rica'),(8,'UCIMED','Universidad de Ciencias Medicas'),(9,'CUC','Colegio Universitario de Cartago'),(10,'CUNA','Colegio Universitario de Negocios y Administracion');
/*!40000 ALTER TABLE `institucion_educativa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oferente`
--

DROP TABLE IF EXISTS `oferente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `oferente` (
  `id_oferente` int(11) NOT NULL AUTO_INCREMENT,
  `identificacion` varchar(30) NOT NULL,
  `tipo_identificacion` enum('CEDULA','DIMEX','PASAPORTE') NOT NULL,
  `nombre_completo` varchar(150) NOT NULL,
  `fecha_nacimiento` date NOT NULL,
  `direccion` varchar(300) DEFAULT NULL,
  `id_distrito` int(11) DEFAULT NULL,
  `fecha_registro` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id_oferente`),
  UNIQUE KEY `identificacion` (`identificacion`),
  KEY `id_distrito` (`id_distrito`),
  KEY `idx_oferente_nombre` (`nombre_completo`),
  KEY `idx_oferente_ident` (`identificacion`),
  CONSTRAINT `oferente_ibfk_1` FOREIGN KEY (`id_distrito`) REFERENCES `distrito` (`id_distrito`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oferente`
--

LOCK TABLES `oferente` WRITE;
/*!40000 ALTER TABLE `oferente` DISABLE KEYS */;
INSERT INTO `oferente` VALUES (1,'101230456','CEDULA','Ana Lucía Ramírez Pérez','1990-03-15','Residencial Los Cipreses, casa 12',1,'2026-05-28 09:32:05'),(2,'205670123','CEDULA','Carlos Andrés Mora Jiménez','1988-07-22','Barrio Los Jardines, apto 3B',6,'2026-05-28 09:32:05'),(3,'304560789','CEDULA','Sofía Valeria Castillo Ulate','1995-11-08','Urb El Roble, casa 45',2,'2026-05-28 09:32:05'),(4,'401890234','CEDULA','Diego Fabricio Vargas Quesada','1992-05-30','Condominio Las Palmas, apto 7',11,'2026-05-28 09:32:05'),(5,'506780123','CEDULA','Valentina Soto Herrera','1987-09-14','Calle 5, av 2, casa esquinera',3,'2026-05-28 09:32:05'),(6,'602340567','CEDULA','Andrés Felipe Núñez Araya','1994-02-28','Barrio Escalante, calle 33',4,'2026-05-28 09:32:05'),(7,'708901234','CEDULA','Gabriela María Acuña León','1991-12-03','Los Yoses, 150m norte del parque',5,'2026-05-28 09:32:05'),(8,'804560123','CEDULA','Roberto Carlos Fonseca Rojas','1985-04-17','La Uruca, frente al hospital',7,'2026-05-28 09:32:05'),(9,'1-2345-6789','CEDULA','María José Blanco Solano','1998-08-25','Santa Ana centro, casa azul',8,'2026-05-28 09:32:05'),(10,'PE123456','PASAPORTE','James Alexander Smith','1989-06-10','Escazú, cond. luxury 101',13,'2026-05-28 09:32:05'),(11,'DIMEX567890','DIMEX','Chen Wei Rodriguez','1993-01-20','Pavas, residencia norte',9,'2026-05-28 09:32:05'),(12,'901234567','CEDULA','Luciana Fernández Castro','1996-07-04','Moravia, barrio La Trinidad',10,'2026-05-28 09:32:05'),(13,'112340678','CEDULA','Daniel Esteban Mora Vega','1990-10-19','Curridabat, urbanización El Sol',4,'2026-05-28 09:32:05'),(14,'213450789','CEDULA','Patricia Elena Segura Durán','1983-03-07','Tibás, barrio La Unión',5,'2026-05-28 09:32:05'),(15,'314560890','CEDULA','Mauricio Alonso Ruiz Brenes','1997-05-22','Goicoechea, San Juan',6,'2026-05-28 09:32:05');
/*!40000 ALTER TABLE `oferente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oferente_concurso`
--

DROP TABLE IF EXISTS `oferente_concurso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `oferente_concurso` (
  `id_oferente` int(11) NOT NULL,
  `id_concurso` int(11) NOT NULL,
  PRIMARY KEY (`id_oferente`,`id_concurso`),
  KEY `id_concurso` (`id_concurso`),
  CONSTRAINT `oferente_concurso_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`),
  CONSTRAINT `oferente_concurso_ibfk_2` FOREIGN KEY (`id_concurso`) REFERENCES `concurso` (`id_concurso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oferente_concurso`
--

LOCK TABLES `oferente_concurso` WRITE;
/*!40000 ALTER TABLE `oferente_concurso` DISABLE KEYS */;
INSERT INTO `oferente_concurso` VALUES (1,1),(1,5),(2,1),(2,6),(3,2),(3,6),(4,3),(4,7),(5,2),(5,5),(6,4),(6,7),(7,1),(7,5),(8,3),(9,6),(10,5),(11,7),(12,6),(13,7),(14,3),(15,5);
/*!40000 ALTER TABLE `oferente_concurso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oferente_correo`
--

DROP TABLE IF EXISTS `oferente_correo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `oferente_correo` (
  `id_correo` int(11) NOT NULL AUTO_INCREMENT,
  `id_oferente` int(11) NOT NULL,
  `correo` varchar(150) NOT NULL,
  PRIMARY KEY (`id_correo`),
  KEY `id_oferente` (`id_oferente`),
  CONSTRAINT `oferente_correo_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oferente_correo`
--

LOCK TABLES `oferente_correo` WRITE;
/*!40000 ALTER TABLE `oferente_correo` DISABLE KEYS */;
INSERT INTO `oferente_correo` VALUES (1,1,'ana.ramirez@gmail.com'),(2,1,'alucia.ramirez@hotmail.com'),(3,2,'carlos.mora@gmail.com'),(4,3,'sofia.castillo@yahoo.com'),(5,3,'scastillo95@gmail.com'),(6,4,'diego.vargas@gmail.com'),(7,5,'vsoto@outlook.com'),(8,6,'andres.nunez@gmail.com'),(9,7,'gabriela.acuna@gmail.com'),(10,8,'rfonseca@hotmail.com'),(11,9,'mj.blanco@gmail.com'),(12,10,'james.smith@gmail.com'),(13,10,'jsmith.cr@outlook.com'),(14,11,'chen.wei@gmail.com'),(15,12,'luciana.fernandez@gmail.com'),(16,13,'daniel.mora@gmail.com'),(17,14,'patricia.segura@hotmail.com'),(18,15,'mauricio.ruiz@gmail.com');
/*!40000 ALTER TABLE `oferente_correo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oferente_telefono`
--

DROP TABLE IF EXISTS `oferente_telefono`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `oferente_telefono` (
  `id_telefono` int(11) NOT NULL AUTO_INCREMENT,
  `id_oferente` int(11) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  PRIMARY KEY (`id_telefono`),
  KEY `id_oferente` (`id_oferente`),
  CONSTRAINT `oferente_telefono_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oferente_telefono`
--

LOCK TABLES `oferente_telefono` WRITE;
/*!40000 ALTER TABLE `oferente_telefono` DISABLE KEYS */;
INSERT INTO `oferente_telefono` VALUES (1,1,'8888-1234'),(2,1,'2222-5678'),(3,2,'8777-2345'),(4,3,'8666-3456'),(5,4,'8555-4567'),(6,4,'2333-8901'),(7,5,'8444-5678'),(8,6,'8333-6789'),(9,7,'8222-7890'),(10,8,'8111-8901'),(11,9,'7999-9012'),(12,10,'7888-0123'),(13,11,'7777-1234'),(14,12,'7666-2345'),(15,13,'7555-3456'),(16,14,'7444-4567'),(17,15,'7333-5678');
/*!40000 ALTER TABLE `oferente_telefono` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pantalla`
--

DROP TABLE IF EXISTS `pantalla`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pantalla` (
  `id_pantalla` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `modulo` varchar(100) NOT NULL,
  `ruta` varchar(250) NOT NULL,
  `icono` varchar(100) DEFAULT NULL,
  `orden_menu` int(11) DEFAULT 1,
  `visible_menu` tinyint(1) DEFAULT 1,
  `activo` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`id_pantalla`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pantalla`
--

LOCK TABLES `pantalla` WRITE;
/*!40000 ALTER TABLE `pantalla` DISABLE KEYS */;
INSERT INTO `pantalla` VALUES (1,'Roles','Seguridad','/Seguridad/Roles','fa-user-tag',1,1,1),(2,'Modulos','Seguridad','/Seguridad/Modulos','fa-th-large',2,1,1),(3,'Usuarios','Seguridad','/Seguridad/Usuarios','fa-users',3,1,1),(4,'Bitacora','General','/General/Bitacora','fa-history',4,1,1),(5,'Parametros','General','/General/Parametros','fa-sliders-h',5,1,1),(6,'Companias','General','/General/Companias','fa-building',6,1,1),(7,'Carga Ubicacion','General','/General/CargaUbicacion','fa-map-marker',7,1,1),(8,'Instituciones Educativas','General','/General/Instituciones','fa-university',8,1,1),(9,'Oferentes','Oferentes','/Oferentes/Index','fa-id-card',9,1,1),(10,'Concursos','Oferentes','/Oferentes/Concursos','fa-trophy',10,1,1),(11,'Agendar Entrevista','Oferentes','/Oferentes/Entrevistas','fa-calendar-alt',11,1,1),(12,'Contratar Empleado','Empleados','/Empleados/Contratar','fa-user-plus',12,1,1),(13,'Puestos','Empleados','/Empleados/Puestos','fa-briefcase',13,1,1),(14,'Areas','Empleados','/Empleados/Areas','fa-sitemap',14,1,1),(15,'Acciones de Personal','Empleados','/Empleados/Acciones','fa-file-alt',15,1,1),(17,'PrebaRazor','Seguridad','#','fa-window-maximize',99,0,1);
/*!40000 ALTER TABLE `pantalla` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `parametro`
--

DROP TABLE IF EXISTS `parametro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `parametro` (
  `id_parametro` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `valor` varchar(500) NOT NULL,
  PRIMARY KEY (`id_parametro`),
  UNIQUE KEY `codigo` (`codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `parametro`
--

LOCK TABLES `parametro` WRITE;
/*!40000 ALTER TABLE `parametro` DISABLE KEYS */;
INSERT INTO `parametro` VALUES (1,'SESION_MINUTOS','5'),(2,'MAX_INTENTOS_LOGIN','3');
/*!40000 ALTER TABLE `parametro` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `preparacion_academica`
--

DROP TABLE IF EXISTS `preparacion_academica`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `preparacion_academica` (
  `id_preparacion` int(11) NOT NULL AUTO_INCREMENT,
  `id_oferente` int(11) NOT NULL,
  `id_institucion` int(11) NOT NULL,
  `titulo` varchar(100) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  PRIMARY KEY (`id_preparacion`),
  KEY `id_oferente` (`id_oferente`),
  KEY `id_institucion` (`id_institucion`),
  CONSTRAINT `preparacion_academica_ibfk_1` FOREIGN KEY (`id_oferente`) REFERENCES `oferente` (`id_oferente`),
  CONSTRAINT `preparacion_academica_ibfk_2` FOREIGN KEY (`id_institucion`) REFERENCES `institucion_educativa` (`id_institucion`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `preparacion_academica`
--

LOCK TABLES `preparacion_academica` WRITE;
/*!40000 ALTER TABLE `preparacion_academica` DISABLE KEYS */;
INSERT INTO `preparacion_academica` VALUES (1,1,1,'Licenciatura en Medicina General','2008-03-01','2014-11-30'),(2,1,1,'Especialidad en Medicina Interna','2015-01-15','2018-12-15'),(3,2,2,'Bachillerato en Ingenieria en Sistemas','2006-03-01','2011-11-30'),(4,2,2,'Licenciatura en Ingenieria en Sistemas','2012-01-15','2013-11-30'),(5,3,8,'Licenciatura en Enfermeria','2013-03-01','2018-11-30'),(6,4,1,'Bachillerato en Administracion de Empresas','2010-03-01','2014-11-30'),(7,4,6,'Maestria en Recursos Humanos','2015-01-15','2017-06-30'),(8,5,1,'Licenciatura en Enfermeria Especializada','2005-03-01','2010-11-30'),(9,6,3,'Bachillerato en Contabilidad','2012-03-01','2016-11-30'),(10,7,8,'Licenciatura en Medicina General','2009-03-01','2015-11-30'),(11,8,2,'Bachillerato en Seguridad Ocupacional','2003-03-01','2008-11-30'),(12,9,4,'Bachillerato en Farmacia','2016-03-01','2020-11-30'),(13,10,1,'Doctorado en Medicina Especializada','2007-09-01','2013-06-30'),(14,11,2,'Bachillerato en Ingenieria en Computacion','2011-03-01','2015-11-30'),(15,12,3,'Bachillerato en Psicologia','2014-03-01','2018-11-30'),(16,13,9,'Tecnico en Radiologia','2008-03-01','2011-11-30'),(17,14,1,'Licenciatura en Trabajo Social','2001-03-01','2006-11-30'),(18,15,5,'Bachillerato en Informatica Medica','2015-03-01','2019-11-30');
/*!40000 ALTER TABLE `preparacion_academica` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `provincia`
--

DROP TABLE IF EXISTS `provincia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `provincia` (
  `id_provincia` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  PRIMARY KEY (`id_provincia`),
  UNIQUE KEY `nombre` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `provincia`
--

LOCK TABLES `provincia` WRITE;
/*!40000 ALTER TABLE `provincia` DISABLE KEYS */;
INSERT INTO `provincia` VALUES (2,'Alajuela'),(3,'Cartago'),(5,'Guanacaste'),(4,'Heredia'),(7,'Limón'),(6,'Puntarenas'),(1,'San José');
/*!40000 ALTER TABLE `provincia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `puesto`
--

DROP TABLE IF EXISTS `puesto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `puesto` (
  `id_puesto` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `salario` decimal(18,2) NOT NULL,
  `id_area` int(11) NOT NULL,
  `id_puesto_jefe` int(11) DEFAULT NULL,
  PRIMARY KEY (`id_puesto`),
  UNIQUE KEY `codigo` (`codigo`),
  KEY `id_area` (`id_area`),
  KEY `id_puesto_jefe` (`id_puesto_jefe`),
  CONSTRAINT `puesto_ibfk_1` FOREIGN KEY (`id_area`) REFERENCES `area` (`id_area`),
  CONSTRAINT `puesto_ibfk_2` FOREIGN KEY (`id_puesto_jefe`) REFERENCES `puesto` (`id_puesto`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `puesto`
--

LOCK TABLES `puesto` WRITE;
/*!40000 ALTER TABLE `puesto` DISABLE KEYS */;
INSERT INTO `puesto` VALUES (1,'PUE-001','Director General',3500000.00,1,NULL),(2,'PUE-002','Jefe de Recursos Humanos',1800000.00,2,1),(3,'PUE-003','Reclutador de Personal',1200000.00,2,2),(4,'PUE-004','Asistente de RRHH',900000.00,2,2),(5,'PUE-005','Jefe de Medicina General',2800000.00,3,1),(6,'PUE-006','Medico General',2200000.00,3,5),(7,'PUE-007','Medico Especialista',2800000.00,3,5),(8,'PUE-008','Jefe de Enfermería',2000000.00,4,1),(9,'PUE-009','Enfermera Especializada',1600000.00,4,8),(10,'PUE-010','Auxiliar de Enfermería',1000000.00,4,8),(11,'PUE-011','Jefe de TI',2200000.00,5,1),(12,'PUE-012','Desarrollador de Software',1700000.00,5,11),(13,'PUE-013','Soporte Tecnico TI',1100000.00,5,11),(14,'PUE-014','Jefe Administrativo Financiero',2000000.00,6,1),(15,'PUE-015','Contador',1500000.00,6,14),(16,'PUE-016','Asistente Administrativo',950000.00,6,14),(17,'PUE-017','Jefe de Radiología',2200000.00,7,1),(18,'PUE-018','Tecnico en Radiología',1400000.00,7,17),(19,'PUE-019','Jefe de Farmacia',2000000.00,8,1),(20,'PUE-020','Farmaceutico',1800000.00,8,19),(21,'PUE-021','Jefe de Seguridad Ocupacional',1700000.00,9,1),(22,'PUE-022','Inspector de Seguridad',1200000.00,9,21);
/*!40000 ALTER TABLE `puesto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `requisito_puesto`
--

DROP TABLE IF EXISTS `requisito_puesto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `requisito_puesto` (
  `id_requisito` int(11) NOT NULL AUTO_INCREMENT,
  `id_puesto` int(11) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  PRIMARY KEY (`id_requisito`),
  KEY `id_puesto` (`id_puesto`),
  CONSTRAINT `requisito_puesto_ibfk_1` FOREIGN KEY (`id_puesto`) REFERENCES `puesto` (`id_puesto`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `requisito_puesto`
--

LOCK TABLES `requisito_puesto` WRITE;
/*!40000 ALTER TABLE `requisito_puesto` DISABLE KEYS */;
INSERT INTO `requisito_puesto` VALUES (1,1,'Grado de Maestría o superior en Administración de Empresas o afín'),(2,1,'Mínimo 10 años de experiencia en puestos de dirección'),(3,1,'Manejo avanzado de herramientas de gestión empresarial'),(4,2,'Licenciatura en Administración de Empresas o Recursos Humanos'),(5,2,'Mínimo 5 años de experiencia en gestión de personal'),(6,2,'Conocimiento en legislación laboral costarricense'),(7,3,'Bachillerato en Administración de Empresas o afín'),(8,3,'Mínimo 2 años de experiencia en reclutamiento'),(9,6,'Título de Médico General reconocido por el Colegio de Médicos de Costa Rica'),(10,6,'Incorporado al Colegio de Médicos y Cirujanos de Costa Rica'),(11,6,'Mínimo 2 años de experiencia en atención clínica'),(12,7,'Especialidad médica reconocida'),(13,7,'Incorporado al Colegio de Médicos y Cirujanos de Costa Rica'),(14,7,'Mínimo 3 años de experiencia en especialidad'),(15,9,'Licenciatura en Enfermería'),(16,9,'Incorporada al Colegio de Enfermeras de Costa Rica'),(17,9,'Mínimo 3 años de experiencia en área hospitalaria'),(18,11,'Licenciatura en Ingeniería en Sistemas o Computación'),(19,11,'Mínimo 5 años de experiencia en gestión de TI'),(20,11,'Conocimiento en seguridad informática y redes'),(21,12,'Bachillerato en Ingeniería en Sistemas o afín'),(22,12,'Mínimo 2 años de experiencia en desarrollo'),(23,12,'Conocimiento en .NET y bases de datos relacionales'),(24,18,'Técnico en Radiología e Imágenes Médicas'),(25,18,'Incorporado al Colegio Técnico respectivo'),(26,18,'Mínimo 1 año de experiencia en manejo de equipo radiológico');
/*!40000 ALTER TABLE `requisito_puesto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol`
--

DROP TABLE IF EXISTS `rol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol` (
  `id_rol` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(40) NOT NULL,
  `activo` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`id_rol`),
  UNIQUE KEY `nombre` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol`
--

LOCK TABLES `rol` WRITE;
/*!40000 ALTER TABLE `rol` DISABLE KEYS */;
INSERT INTO `rol` VALUES (1,'Administrador',1),(2,'Reclutador',1),(3,'Supervisor',1),(4,'Jefatura',1),(6,'Profesor',1),(7,'Recursos Humanos',1),(8,'Encargado',1),(10,'Sin asociación',1),(11,'Estudiante',1),(12,'Limpieza',1),(13,'Prueba con asociacion',1),(14,'PruebaRazor',1);
/*!40000 ALTER TABLE `rol` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol_pantalla`
--

DROP TABLE IF EXISTS `rol_pantalla`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol_pantalla` (
  `id_rol` int(11) NOT NULL,
  `id_pantalla` int(11) NOT NULL,
  PRIMARY KEY (`id_rol`,`id_pantalla`),
  KEY `id_pantalla` (`id_pantalla`),
  CONSTRAINT `rol_pantalla_ibfk_1` FOREIGN KEY (`id_rol`) REFERENCES `rol` (`id_rol`),
  CONSTRAINT `rol_pantalla_ibfk_2` FOREIGN KEY (`id_pantalla`) REFERENCES `pantalla` (`id_pantalla`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol_pantalla`
--

LOCK TABLES `rol_pantalla` WRITE;
/*!40000 ALTER TABLE `rol_pantalla` DISABLE KEYS */;
INSERT INTO `rol_pantalla` VALUES (1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,9),(1,10),(1,11),(1,12),(1,13),(1,14),(1,15),(2,8),(2,9),(2,10),(2,11),(2,12),(2,13),(2,14),(2,15),(3,3),(3,8),(3,10),(3,11),(3,12),(3,13),(3,15),(4,3),(4,4),(4,15),(6,8),(6,10),(6,11),(7,1),(7,3),(7,4),(7,9),(7,12),(7,13),(7,14),(7,15),(8,3),(8,11),(11,10),(11,11),(12,14),(13,4),(13,13),(14,11),(14,17);
/*!40000 ALTER TABLE `rol_pantalla` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `id_usuario` int(11) NOT NULL AUTO_INCREMENT,
  `usuario` varchar(50) NOT NULL,
  `nombre_completo` varchar(150) NOT NULL,
  `correo` varchar(150) NOT NULL,
  `password_hash` varchar(500) NOT NULL,
  `estado` enum('ACTIVO','INACTIVO','BLOQUEADO') DEFAULT 'ACTIVO',
  `intentos_login` int(11) DEFAULT 0,
  `fecha_creacion` datetime DEFAULT current_timestamp(),
  `fecha_ultimo_login` datetime DEFAULT NULL,
  `fecha_bloqueo` datetime DEFAULT NULL,
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `usuario` (`usuario`),
  UNIQUE KEY `correo` (`correo`),
  KEY `idx_usuario_login` (`usuario`),
  KEY `idx_usuario_correo` (`correo`),
  KEY `idx_usuario_estado` (`estado`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'admin','Administrador Sistema','admin@serviciosmedicos.com','AESGCM:GnZt95gP/RoxN1l1:7DubipsjrkXKsIDj7vjTlw==:ndclS8Q37hqA','ACTIVO',0,'2026-05-28 09:32:03','2026-06-03 00:25:23',NULL),(2,'mfernandez','María Fernández Rojas','mfernandez@serviciosmedicos.com','Reclu2024*','ACTIVO',0,'2026-05-28 09:32:03','2026-05-28 11:46:57',NULL),(3,'jperez','Juan Pérez Solano','jperez@serviciosmedicos.com','Reclu2024*','ACTIVO',0,'2026-05-28 09:32:03',NULL,NULL),(4,'lmendoza','Laura Mendoza Castro','lmendoza@serviciosmedicos.com','Reclu2024*','ACTIVO',0,'2026-05-28 09:32:03',NULL,NULL),(5,'kgomez','Kevin Gómez Vargas','kgomez@serviciosmedicos.com','Reclu2024*','INACTIVO',0,'2026-05-28 09:32:03',NULL,NULL),(6,'rsanchez','Roberto Sánchez Mora','rsanchez@serviciosmedicos.com','Bloq2024*','BLOQUEADO',0,'2026-05-28 09:32:03',NULL,NULL),(7,'Dencel','Dencel Rodríguez Solano','dencel@gmail.com','AESGCM:ivyEgQpfCVFLWsy3:FpiSRNUQhniWJn/imVgxfA==:GWygR28HDxZIHQ==','ACTIVO',0,'2026-05-28 13:35:13','2026-06-03 14:01:44',NULL),(8,'prueba','prueba','prueba@gmail.com','AESGCM:f/DtYkH1g68fNNCN:ilmmc2+gRJsi+T6gb/oMxw==:PS9nOgIoSaUTL19r','ACTIVO',0,'2026-05-28 13:35:51',NULL,NULL),(10,'ffffdfs','asadadsa','asdasd@gmail.com','AESGCM:f3JpW9DzXzkdJbtU:60Pkf0Cy+w472tyZrs5kVQ==:0/q+GtKrCdu9','ACTIVO',0,'2026-05-28 13:36:42',NULL,NULL),(11,'hfgdhhdh','hdfhdfhdhf','dddd@gmail.com','AESGCM:t27FvMYaNjkGUmcF:O/JwYkXbKM/cc0gNSkJk3w==:piYG5Ap3z6jD50T3','ACTIVO',0,'2026-05-28 13:37:08',NULL,NULL),(12,'Juan Perez','Juan Pérez Villalta','juanperez@gmial.com','AESGCM:VwUIGzFjkWnIKJt7:3kAA0gL18ZNc8gUjHbTdLw==:mrNiFhske9ntc38=','ACTIVO',0,'2026-05-28 15:41:38','2026-06-03 00:26:58',NULL),(13,'Andrew','Andrew Rivera Gamboa','andrew@gmail.com','AESGCM:dA8FDd4E0uS/Y9Tx:BlmzhDzRDTUZsJ39qHZcFA==:oE2nj2IwDwd7iQ==','ACTIVO',0,'2026-05-31 14:42:03','2026-06-03 16:01:22',NULL),(14,'PruebaRazor','aaa','aaa@gmail.com','AESGCM:6L+K+Deounjb31HV:yq6jOHKk5HI/5v4YCspC+g==:eOpMzhThHElN','ACTIVO',0,'2026-06-03 00:24:06',NULL,NULL);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario_rol`
--

DROP TABLE IF EXISTS `usuario_rol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario_rol` (
  `id_usuario` int(11) NOT NULL,
  `id_rol` int(11) NOT NULL,
  PRIMARY KEY (`id_usuario`,`id_rol`),
  KEY `id_rol` (`id_rol`),
  CONSTRAINT `usuario_rol_ibfk_1` FOREIGN KEY (`id_usuario`) REFERENCES `usuario` (`id_usuario`),
  CONSTRAINT `usuario_rol_ibfk_2` FOREIGN KEY (`id_rol`) REFERENCES `rol` (`id_rol`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario_rol`
--

LOCK TABLES `usuario_rol` WRITE;
/*!40000 ALTER TABLE `usuario_rol` DISABLE KEYS */;
INSERT INTO `usuario_rol` VALUES (1,1),(2,2),(3,2),(4,2),(5,2),(6,2),(7,1),(8,10),(10,10),(11,10),(12,2),(13,1);
/*!40000 ALTER TABLE `usuario_rol` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-03 16:18:17
