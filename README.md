# ShadiConnect Mobile App

.NET MAUI demo application for the ShadiTest CRUD API.

## Branch

`mobile-app-test`

## API

The app consumes the existing Users API:

`http://192.168.31.212:8080/api/Users`

The Android manifest allows clear-text HTTP because the current IIS demo API is hosted over HTTP on the local network.

## Features

- Load users from API
- Create profile
- Edit profile
- Delete profile
- Pull-to-refresh
- Basic client-side validation
- Connection error handling
- Responsive matrimonial-style UI

## User model

The mobile model intentionally matches the existing API model: `Id`, `Name`, `Email`, and `Age`. The create request does **not** send `Id`; SQL Server generates the identity value.

## Local demo requirement

For a physical Android phone, the phone and the IIS machine must be on the same LAN, and Windows Firewall must allow inbound TCP traffic on port `8080`.
