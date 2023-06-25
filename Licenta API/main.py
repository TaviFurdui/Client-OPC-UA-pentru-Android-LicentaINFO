import csv
import pandas as pd
import spacy
import numpy as np
import joblib
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score

# Open the CSV file
with open('dataset.csv', 'r') as file:
    reader = csv.reader(file)
    header = next(reader)  # Read the header row

    # Specify the columns you want to extract
    columns_to_extract = ['Query', 'Intent', 'NodeName']

    # Initialize an empty dictionary to store the data
    data = {column: [] for column in columns_to_extract}

    # Iterate over each row and extract the desired columns
    for row in reader:
        for column in columns_to_extract:
            data[column].append(row[header.index(column)])

# Create a pandas DataFrame from the extracted data
df = pd.DataFrame(data)
queries = df['Query'].values
intents = df['Intent'].values
nodes = df['NodeName'].values

# Prepare the labeled dataset (X: queries, y: intents)
X = queries
y = intents

# Split the dataset into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Text preprocessing and feature extraction
vectorizer = TfidfVectorizer()
X_train = vectorizer.fit_transform(X_train)
X_test = vectorizer.transform(X_test)

# Train the logistic regression model
model = LogisticRegression()
model.fit(X_train, y_train)

# Make predictions on the testing set
y_pred = model.predict(X_test)
joblib.dump(model, 'model.pkl')
# Calculate accuracy of the model
accuracy = accuracy_score(y_test, y_pred)
print("Accuracy:", accuracy)



# Load the trained model
model = joblib.load('model.pkl')
new_phrase = "Read value from pressure node"
new_phrase_vectorized = vectorizer.transform([new_phrase])
prediction = model.predict(new_phrase_vectorized)
print(prediction[0])
if prediction[0] == 'Read':
    print("It is read.")
else:
    print("It is write.")