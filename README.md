# Citi Bike Data Analysis

A C# console application for exploring usage patterns in the January 2023 NYC Citi Bike trip dataset. The program reads one million trip records from a CSV file, filters incomplete or malformed records, and uses LINQ to calculate usage statistics and station-level imbalances.

This project was created as an academic data-processing exercise focused on file parsing, data validation, collection processing, and LINQ queries.

## Dataset

The analysis uses `202301-citibike-tripdata_1.csv`:

- **Period:** January 2023
- **Size:** approximately 195 MB
- **Records:** 1,000,000 trips, excluding the header
- **Source:** [Citi Bike System Data](https://citibikenyc.com/system-data)
- **Repository storage:** Git LFS

Each parsed trip contains:

- start and end timestamps;
- start and end station names;
- user type (`member` or `casual`).

## Implemented Analysis

### 1. Peak hours by user type

Trips are grouped by user type and starting hour. The program returns the ten busiest user-type/hour combinations.

### 2. Stations with the longest average trip duration

For stations with more than 500 starting trips, the program calculates the average trip duration and displays the ten highest results.

### 3. Weekday and weekend activity

Trips are divided into weekday and weekend groups to compare their total activity.

### 4. Station imbalance

For each starting station, the program compares the number of trips starting and ending there:

```text
balance = number of starts - number of ends
```

The ten stations with the largest absolute imbalance are displayed. This can indicate locations where the redistribution of bicycles may be needed.

### 5. Average trip duration by time of day

Trips are grouped into four time ranges:

| Time range | Hours |
|---|---|
| Night | 00:00-05:59 |
| Morning | 06:00-11:59 |
| Day | 12:00-17:59 |
| Evening | 18:00-23:59 |

The program calculates the average trip duration for each range.

## Data Processing

The application:

1. reads the dataset line by line with `StreamReader`;
2. skips rows with fewer than 13 fields;
3. validates timestamps with `DateTime.TryParse`;
4. maps valid rows to `Trip` objects;
5. removes records without start or end station names;
6. performs grouping, filtering, ordering, and aggregation with LINQ.

## Technologies

- C#
- .NET
- LINQ
- `StreamReader`
- CSV data
- Git LFS

## Project Structure

| File | Description |
|---|---|
| `Program.cs` | Loads, filters, analyzes, and prints trip data |
| `Trip.cs` | Defines the trip data model |
| `202301-citibike-tripdata_1.csv` | January 2023 trip dataset stored through Git LFS |

## Running the Project

### Requirements

- .NET SDK
- Git LFS

Clone the repository and download the dataset:

```bash
git clone https://github.com/anniigr/Project_proga.git
cd Project_proga
git lfs install
git lfs pull
```
