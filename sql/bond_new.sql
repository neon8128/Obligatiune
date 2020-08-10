-- phpMyAdmin SQL Dump
-- version 5.0.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Aug 10, 2020 at 04:27 PM
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
-- Database: `bond_new`
--

-- --------------------------------------------------------

--
-- Table structure for table `audit`
--

CREATE TABLE `audit` (
  `audit_id` int(11) NOT NULL,
  `username` varchar(45) DEFAULT NULL,
  `TS` datetime DEFAULT NULL,
  `details` varchar(45) DEFAULT NULL,
  `machine_name` varchar(45) DEFAULT NULL,
  `ip` varchar(45) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `bond`
--

CREATE TABLE `bond` (
  `name` varchar(40) NOT NULL,
  `version` int(11) NOT NULL DEFAULT 1,
  `audit_id` int(11) DEFAULT NULL,
  `username` varchar(35) DEFAULT NULL,
  `interest_rate` double DEFAULT NULL,
  `ccy` varchar(45) DEFAULT NULL,
  `principal` int(11) DEFAULT NULL,
  `day_counting_convention` varchar(45) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `bond_hist`
--

CREATE TABLE `bond_hist` (
  `audit_id` int(11) DEFAULT NULL,
  `username` varchar(45) DEFAULT NULL,
  `name` varchar(40) NOT NULL,
  `version` int(11) NOT NULL DEFAULT 1,
  `interest_rate` double DEFAULT NULL,
  `ccy` varchar(45) DEFAULT NULL,
  `principal` int(11) DEFAULT NULL,
  `day_counting_convention` varchar(45) DEFAULT 'ACT/365',
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `fxr`
--

CREATE TABLE `fxr` (
  `name` varchar(20) NOT NULL,
  `version` double NOT NULL DEFAULT 1,
  `as_of_date` date NOT NULL,
  `ccy1` varchar(5) NOT NULL,
  `ccy2` varchar(5) NOT NULL,
  `term` varchar(11) NOT NULL DEFAULT 'SPOT',
  `rate` double NOT NULL,
  `audit_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `fxr_hist`
--

CREATE TABLE `fxr_hist` (
  `version` double NOT NULL DEFAULT 1,
  `as_of_date` date NOT NULL,
  `ccy1` varchar(5) NOT NULL,
  `ccy2` varchar(5) NOT NULL,
  `term` varchar(11) NOT NULL DEFAULT 'SPOT',
  `rate` double NOT NULL,
  `audit_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `interest_rate`
--

CREATE TABLE `interest_rate` (
  `name` varchar(45) NOT NULL,
  `version` int(11) NOT NULL DEFAULT 1,
  `as_of_date` date NOT NULL,
  `term` varchar(12) NOT NULL,
  `date` date NOT NULL,
  `audit_id` int(11) NOT NULL,
  `rate` double NOT NULL,
  `ccy` varchar(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `interest_rate_hist`
--

CREATE TABLE `interest_rate_hist` (
  `name` varchar(45) NOT NULL,
  `version` int(11) NOT NULL,
  `as_of_date` date NOT NULL,
  `term` varchar(12) NOT NULL,
  `date` date NOT NULL,
  `rate` double NOT NULL,
  `ccy` varchar(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `schedule`
--

CREATE TABLE `schedule` (
  `bond_name` varchar(11) NOT NULL,
  `ref_day` date NOT NULL,
  `coupon_day` date NOT NULL,
  `audit_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `username` varchar(45) NOT NULL,
  `email` varchar(45) NOT NULL,
  `first_name` varchar(45) DEFAULT NULL,
  `last_name` varchar(45) DEFAULT NULL,
  `password` varchar(90) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `version` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `user_hist`
--

CREATE TABLE `user_hist` (
  `version` int(11) NOT NULL,
  `username` varchar(45) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `password` varchar(32) NOT NULL,
  `first_name` varchar(45) DEFAULT NULL,
  `last_name` varchar(45) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `audit`
--
ALTER TABLE `audit`
  ADD PRIMARY KEY (`audit_id`),
  ADD KEY `username` (`username`);

--
-- Indexes for table `bond`
--
ALTER TABLE `bond`
  ADD PRIMARY KEY (`name`),
  ADD KEY `audit_id` (`audit_id`) USING BTREE;

--
-- Indexes for table `bond_hist`
--
ALTER TABLE `bond_hist`
  ADD KEY `audit_id_bond_hist` (`audit_id`);

--
-- Indexes for table `fxr`
--
ALTER TABLE `fxr`
  ADD PRIMARY KEY (`name`),
  ADD KEY `audit_id` (`audit_id`);

--
-- Indexes for table `interest_rate`
--
ALTER TABLE `interest_rate`
  ADD PRIMARY KEY (`name`,`as_of_date`,`term`,`date`),
  ADD KEY `audit_id` (`audit_id`);

--
-- Indexes for table `interest_rate_hist`
--
ALTER TABLE `interest_rate_hist`
  ADD KEY `name` (`name`,`as_of_date`,`term`,`date`);

--
-- Indexes for table `schedule`
--
ALTER TABLE `schedule`
  ADD KEY `bond_name_schedule` (`bond_name`),
  ADD KEY `audit_id` (`audit_id`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`username`);

--
-- Indexes for table `user_hist`
--
ALTER TABLE `user_hist`
  ADD KEY `username_idx` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `audit`
--
ALTER TABLE `audit`
  MODIFY `audit_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `audit`
--
ALTER TABLE `audit`
  ADD CONSTRAINT `username` FOREIGN KEY (`username`) REFERENCES `user` (`username`) ON DELETE SET NULL ON UPDATE SET NULL;

--
-- Constraints for table `bond`
--
ALTER TABLE `bond`
  ADD CONSTRAINT `audit_bond` FOREIGN KEY (`audit_id`) REFERENCES `audit` (`audit_id`);

--
-- Constraints for table `bond_hist`
--
ALTER TABLE `bond_hist`
  ADD CONSTRAINT `audit_id_bond_hist` FOREIGN KEY (`audit_id`) REFERENCES `audit` (`audit_id`);

--
-- Constraints for table `fxr`
--
ALTER TABLE `fxr`
  ADD CONSTRAINT `audit_fxr` FOREIGN KEY (`audit_id`) REFERENCES `audit` (`audit_id`);

--
-- Constraints for table `interest_rate`
--
ALTER TABLE `interest_rate`
  ADD CONSTRAINT `audit_interest` FOREIGN KEY (`audit_id`) REFERENCES `audit` (`audit_id`);

--
-- Constraints for table `interest_rate_hist`
--
ALTER TABLE `interest_rate_hist`
  ADD CONSTRAINT `interest_con` FOREIGN KEY (`name`,`as_of_date`,`term`,`date`) REFERENCES `interest_rate` (`name`, `as_of_date`, `term`, `date`) ON UPDATE NO ACTION;

--
-- Constraints for table `schedule`
--
ALTER TABLE `schedule`
  ADD CONSTRAINT `audit_id` FOREIGN KEY (`audit_id`) REFERENCES `audit` (`audit_id`),
  ADD CONSTRAINT `bond_name_schedule` FOREIGN KEY (`bond_name`) REFERENCES `bond` (`name`);

--
-- Constraints for table `user_hist`
--
ALTER TABLE `user_hist`
  ADD CONSTRAINT `username_hist` FOREIGN KEY (`username`) REFERENCES `user` (`username`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
