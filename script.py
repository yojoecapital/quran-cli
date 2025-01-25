import csv

def modify_csv(input_file, output_file):
    # Open the input CSV file in read mode
    with open(input_file, mode='r', newline='') as infile:
        reader = csv.reader(infile)
        rows = list(reader)

    # Modify the second column, excluding the last row
    for row in rows[:-1]:  # Skip the last row
        try:
            # Subtract 1 from the second column value
            row[1] = str(int(row[1]) - 1)
        except ValueError:
            print(f"Warning: Non-integer value encountered in second column: {row[1]}")
            continue

    # Write the modified data to the output CSV file
    with open(output_file, mode='w', newline='') as outfile:
        writer = csv.writer(outfile)
        writer.writerows(rows)

# Example usage
input_file = 'pages.csv'  # Path to the input CSV file
output_file = 'modified_pages.csv'  # Path to the output CSV file
modify_csv(input_file, output_file)

print(f"CSV modified and saved to {output_file}")
