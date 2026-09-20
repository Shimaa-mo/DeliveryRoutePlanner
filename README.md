# Delivery Route Planner

A C# console application that plans delivery trips while respecting vehicle capacity, delivery priority, and delivery area.

## Features

* Reads delivery requests from a text file.
* Groups deliveries by area where possible.
* Prioritizes deliveries using priority numbers, where a lower number means higher priority.
* Ensures that no trip exceeds the 10 kg maximum capacity.
* Handles deliveries that exceed the maximum package weight.
* Allows new deliveries to be scheduled after the initial planning.
* Rebalances existing trips when a new delivery is added.
* Displays the current trips and their remaining capacity.
* Automatically reorganizes trip IDs after trips are added or removed.

---

# Input Format

The application reads deliveries from a text file.

Each line represents one delivery using the following format:

```text
ID|Area|Priority|PackageWeight
```

Example:

```text
1|Cairo|1|4.5
2|Giza|2|3
3|Cairo|2|5
4|Alexandria|1|7
5|Cairo|3|2
```

The fields are:

* **ID**: Unique delivery identifier.
* **Area**: Delivery destination area.
* **Priority**: Lower number means higher priority.
* **PackageWeight**: Weight of the package in kilograms.

A sample input file is included as `test.txt`.

---

# Project Structure

```text
DeliveryRoutePlanner/                  <-- Git repository root
│
├── .git/
├── .gitignore
├── README.md
├── test.txt                            <-- Sample input file
│
└── DeliveryRoutePlanner/              <-- Visual Studio solution folder
    │
    ├── .vs/
    ├── eT3_assignment.sln
    │
    └── eT3_assignment/                <-- C# project folder
        │
        ├── Program.cs
        ├── Delivery.cs
        ├── Trip.cs
        ├── TripPlanner.cs
        ├── PackageWeightExceededException.cs
        └── TripFullException.cs
```

### Program.cs

Handles:

* User interaction.
* Reading the input file.
* Validating user input.
* Asking for new delivery information.
* Displaying the current trips.
* Allowing the user to schedule new deliveries.

### Delivery.cs

Represents a delivery request and contains its:

* ID
* Area
* Priority
* Package weight

### Trip.cs

Represents a delivery trip and is responsible for:

* Storing deliveries.
* Tracking total weight.
* Calculating free capacity.
* Adding and removing deliveries.
* Checking whether a delivery can fit.
* Enforcing the 10 kg capacity limit.

### TripPlanner.cs

Contains the main planning logic:

* Initial trip creation.
* Selecting deliveries based on priority.
* Grouping deliveries by area where possible.
* Scheduling new deliveries.
* Inserting new trips when necessary.
* Rebalancing deliveries between trips.
* Removing empty trips.
* Reorganizing trip IDs.

### PackageWeightExceededException.cs

Defines a custom exception used when a package exceeds the maximum allowed package weight.

### TripFullException.cs

Defines a custom exception used when attempting to add a delivery to a full trip.

### test.txt

Contains sample delivery data that can be used to run the program.

---

# How to Run

1. Clone the repository.
2. Open `eT3_assignment.sln` in Visual Studio.
3. Build the solution.
4. Run the console application.
5. When prompted, enter the path to the input file, or press enter to use the configured default path, for example:

```text
D:\vs projects\eT3_assignment\DeliveryRoutePlanner\test.txt
```

6. The application will read the deliveries and display the planned trips.

After the initial planning, the menu allows you to:

```text
1. Schedule a new delivery
2. View current trips
3. Exit
```

---

# Planning Approach

The application uses a greedy approach for the initial planning.

For each trip:

1. The highest-priority remaining delivery is selected as the starting point.
2. Its area becomes the area of the current trip.
3. The planner looks for deliveries from the same area.
4. Among them, the highest-priority delivery that can fit is selected.
5. Deliveries are added while respecting the 10 kg capacity.
6. When no more suitable delivery can be added, the trip is completed and a new trip is started.

This approach attempts to keep deliveries from the same area together while giving priority to more urgent deliveries.

---

# Scheduling New Deliveries

When a new delivery is added after the initial planning, the planner first checks whether it can fit into an existing trip from the same area without violating the priority ordering.

If that is not possible:

* A new trip is created.
* The new trip is inserted according to priority.
* Existing deliveries may be moved between trips through the rebalancing process.
* Empty trips are removed.
* Trip IDs are reorganized.

This helps maintain a reasonable grouping of areas while making use of available capacity in existing trips.

---

# Edge Cases

The application handles the following cases:

### Empty input file

If the input file contains no deliveries, the program informs the user and exits.

### Package exceeds 10 kg

A package heavier than the vehicle's maximum capacity cannot be scheduled.

All such deliveries are collected and displayed together in a warning after the planning process.

### Package does not fit in the current trip

If the next delivery would make the trip exceed 10 kg, it is skipped temporarily so that another suitable delivery can be considered.

### Same priority

Deliveries with the same priority can still be scheduled normally. The planner uses the available capacity and area grouping to determine their placement.

---

# Extra Feature

An additional feature was implemented for scheduling **new deliveries after the initial planning**.

Instead of simply creating a new trip for every new delivery, the application attempts to:

* Use available capacity in existing trips.
* Keep the delivery with the appropriate area.
* Insert new trips according to priority.
* Rebalance deliveries between trips when possible.

This makes the application interactive rather than only processing the initial input file.

---

# Most Difficult Part

The most challenging part was implementing the **new delivery scheduling and rebalancing logic**.

Adding a delivery is not only about finding an empty space. The new delivery may have a higher priority than deliveries already assigned to trips. Therefore, adding it can require:

* Creating a new trip.
* Moving deliveries between trips.
* Updating trip priorities.
* Removing trips that become empty.
* Reassigning trip IDs.

The rebalancing process was implemented to make better use of existing capacity while keeping the trip organization reasonable.

---

# Limitations and Non-Optimal Cases

The initial planning algorithm is a greedy approach, so it does not guarantee the mathematically optimal number of trips.

For example, choosing the highest-priority delivery first may leave unused capacity that could have been used more efficiently with a different combination of deliveries.

The algorithm prioritizes:

1. Delivery priority.
2. Same-area grouping.
3. Available trip capacity.

This makes the solution understandable and practical for the assignment, but it may not always produce the minimum possible number of trips.

---

# Performance Considerations

For a small or moderate number of deliveries, the current approach is sufficient and keeps the implementation simple.

With around 1 million delivery requests, several parts could become expensive:

* Keeping all deliveries in memory using `List<Delivery>`.
* Repeated LINQ operations such as `Where`, `MinBy`, and `OrderBy`.
* Creating temporary lists using `ToList()`.
* Rebalancing trips by repeatedly searching through their deliveries.
* Removing items from lists, which can require shifting elements.

The application could therefore become slower and consume significant memory with very large inputs.

---

# Possible Improvements

With another day of development, I would improve the application by:

* Adding automated unit tests.
* Separating input parsing and validation from the main program flow.
* Improving the data structures used for large numbers of deliveries.
* Reducing repeated LINQ searches.
* Improving the trip optimization algorithm.
* Adding more detailed validation and error handling.
* Adding performance testing with large input files.
* Improving the user interface and reporting.

---

# Requirements Summary

The application satisfies the main assignment requirements by:

* Representing deliveries with ID, area, priority, and package weight.
* Enforcing a maximum trip capacity of 10 kg.
* Avoiding capacity violations.
* Grouping deliveries by area where possible.
* Prioritizing lower priority numbers.
* Ensuring valid deliveries are scheduled once.
* Reading deliveries from a file.
* Handling overweight packages.
* Supporting new deliveries after the initial planning.
* Providing a sample input file.
* Documenting the approach and its limitations.
