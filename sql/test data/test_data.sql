-- phpMyAdmin SQL Dump
-- version 5.0.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jul 30, 2020 at 04:16 PM
-- Server version: 10.4.13-MariaDB
-- PHP Version: 7.4.8







SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bond`
--

--
-- Dumping data for table `audit`
--

INSERT INTO `audit` (`audit_id`, `username`, `TS`, `details`, `machine_name`, `ip`) VALUES
(1, 'david06', '2020-07-23 00:00:00', 'bond', 'pc', '192.168.0.1'),
(3, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(4, 'admin', '2020-07-28 20:20:16', 'User created', NULL, NULL),
(22, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(23, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(24, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(25, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(26, 'david06', '0000-00-00 00:00:00', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(27, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(28, 'david06', '0000-00-00 00:00:00', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(30, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(31, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(32, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(33, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(34, 'david06', '0000-00-00 00:00:00', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(35, 'Costy', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(36, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(37, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(38, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(39, 'david06', '0000-00-00 00:00:00', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(40, 'david06', '2020-07-28 19:33:36', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(41, 'david06', '2020-07-28 19:59:51', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(42, 'david06', '2020-07-28 21:55:09', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(43, 'david06', '2020-07-29 14:05:58', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(44, 'admin', '2020-07-29 14:12:01', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(45, 'david06', '2020-07-29 14:12:17', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(46, 'david06', '2020-07-29 14:32:40', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(47, 'david06', '2020-07-29 14:39:12', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(48, 'david06', '2020-07-29 15:46:53', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(49, 'david06', '2020-07-29 15:46:54', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(50, 'david06', '2020-07-29 21:42:10', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(51, 'david06', '2020-07-29 21:42:11', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(52, 'david06', '2020-07-30 12:40:58', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(53, 'david06', '2020-07-30 12:47:38', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(54, 'david06', '2020-07-30 14:35:45', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(55, 'david06', '2020-07-30 14:40:35', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(56, 'david06', '2020-07-30 14:41:10', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(57, 'david06', '2020-07-30 14:50:54', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(58, 'admin', '2020-07-30 15:10:50', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(59, 'admin', '2020-07-30 15:16:41', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(60, 'admin', '2020-07-30 15:17:58', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(61, 'admin', '2020-07-30 16:20:42', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(62, NULL, '2020-07-30 16:21:00', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(63, 'admin', '2020-07-30 16:21:36', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(64, 'admin', '2020-07-30 16:21:54', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(65, 'david06', '2020-07-30 16:23:53', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(66, 'david06', '2020-07-30 16:24:09', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(67, 'david06', '2020-07-30 16:26:49', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(68, 'david06', '2020-07-30 16:27:07', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(69, 'david06', '2020-07-30 16:27:50', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(70, 'david06', '2020-07-30 16:28:49', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(71, 'Costy', '2020-07-30 16:30:44', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(72, 'Costy', '2020-07-30 16:31:02', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(73, 'Costy', '2020-07-30 16:31:21', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(74, 'david06', '2020-07-30 16:42:50', 'user logged in', 'DESKTOP-0HGIVFC', '192.168.0.107'),
(75, 'david06', '2020-07-30 16:43:16', 'bond', 'DESKTOP-0HGIVFC', '192.168.0.107');

--
-- Dumping data for table `bond`
--

INSERT INTO `bond` (`name`, `version`, `audit_id`, `interest_rate`, `ccy`, `principal`, `day_counting_convention`, `start_date`, `end_date`) VALUES
('', 1, 49, 0, '', 0, 'ACT/365', '0000-00-00', '0000-00-00'),
('bond1', 3, 1, 1.8, 'eur', 1000, 'ACT/365', '2020-07-10', '2020-10-17'),
('bond2', 1, 1, 2, 'ron', 987, 'ACT/365', '2020-07-28', '2021-01-29'),
('NewTest', 1, 75, 2.2, 'eur', 100, 'ACT/365', '0000-00-00', '0000-00-00'),
('Tesst', 1, 73, 2.3, 'eur', 2, 'ACT/365', '0000-00-00', '0000-00-00'),
('test', 1, 26, 1, 'ron', 1000, 'ACT/365', '0000-00-00', '0000-00-00'),
('test2', 1, 26, 3, 'ron', 1000, 'ACT/365', '0000-00-00', '0000-00-00');

--
-- Dumping data for table `bond_hist`
--

INSERT INTO `bond_hist` (`hist_id`, `audit_id`, `bond_name`, `version`, `interest_rate`, `ccy`, `principal`, `day_counting_convention`, `start_date`, `end_date`) VALUES
(1, 1, 'bond1', 1, 1.8, 'eur', 1000, 'ACT/365', '2020-07-10', '2020-10-17'),
(6, 1, 'bond1', 2, 1.7, 'eur', 1000, 'ACT/365', '2020-07-10', '2020-10-17'),
(7, 1, 'bond1', 3, 1.8, 'eur', 1000, 'ACT/365', '2020-07-10', '2020-10-17'),
(8, 1, 'bond2', 1, 2, 'ron', 987, 'ACT/365', '2020-07-28', '2021-01-29'),
(11, 26, 'test', 1, 1, 'ron', 1000, 'ACT/365', '0000-00-00', '0000-00-00'),
(12, 26, 'test2', 1, 3, 'ron', 1000, 'ACT/365', '0000-00-00', '0000-00-00'),
(13, 49, '', 1, 0, '', 0, 'ACT/365', '0000-00-00', '0000-00-00'),
(14, 73, 'Tesst', 1, 2.3, 'eur', 2, 'ACT/365', '0000-00-00', '0000-00-00'),
(15, 75, 'NewTest', 1, 2.2, 'eur', 100, 'ACT/365', '0000-00-00', '0000-00-00');

--
-- Dumping data for table `interest_rate`
--

INSERT INTO `interest_rate` (`name`, `version`, `as_of_date`, `term`, `date`, `audit_id`, `rate`, `ccy`) VALUES
('bcr', 1, '0000-00-00', '3y', '2020-11-20', 1, 1.3, 'ron'),
('ing', 2, '0000-00-00', '1y', '2021-07-23', 1, 1.5, 'ron');

--
-- Dumping data for table `interest_rate_hist`
--

INSERT INTO `interest_rate_hist` (`interest_hist_id`, `name`, `version`, `as_of_date`, `term`, `date`, `rate`, `ccy`) VALUES
(4, 'ing', 1, '0000-00-00', '1y', '2021-07-23', 1.5, 'ron'),
(5, 'ing', 2, '0000-00-00', '1y', '2021-07-23', 1.5, 'ron'),
(7, 'bcr', 1, '0000-00-00', '3y', '2020-11-20', 1.3, 'ron');

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`username`, `audit_id`, `email`, `first_name`, `last_name`, `password`, `version`) VALUES
('admin', 4, '', NULL, NULL, '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 3),
('Costy', 3, '', NULL, NULL, '4de339ce6a64d7c807b68dc79df5ea2a1608c6b0c577722c389ef43bedc63f97', 3),
('david06', 3, 'suciudavid6@gmail.com', 'david', 'suciu', '6b51d431df5d7f141cbececcf79edf3dd861c3b4069f0b11661a3eefacbba918', 4);

--
-- Dumping data for table `user_hist`
--

INSERT INTO `user_hist` (`id_hist`, `version`, `username`, `email`, `password`, `first_name`, `last_name`) VALUES
(17, 1, 'david06', 'suciudavid6@gmail.com', '1234', 'david', 'suciu'),
(18, 2, 'david06', 'suciudavid6@gmail.com', '123', 'david', 'suciu'),
(19, 3, 'david06', 'suciudavid6@gmail.com', '12', 'david', 'suciu'),
(20, 0, 'Costy', '', 'MyApp', NULL, NULL),
(21, 0, 'david06', 'suciudavid6@gmail.com', '12', 'david', 'suciu'),
(22, 0, 'Costy', '', 'MyApp', NULL, NULL),
(23, 0, 'admin', '', 'admin', NULL, NULL),
(24, 0, 'admin', '', 'admin', NULL, NULL),
(25, 0, 'david06', 'suciudavid6@gmail.com', '6b51d431df5d7f141cbececcf79edf3d', 'david', 'suciu'),
(26, 0, 'david06', 'suciudavid6@gmail.com', '6b51d431df5d7f141cbececcf79edf3d', 'david', 'suciu'),
(27, 0, 'Costy', '', '4de339ce6a64d7c807b68dc79df5ea2a', NULL, NULL),
(28, 0, 'admin', '', '8c6976e5b5410415bde908bd4dee15df', NULL, NULL);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
