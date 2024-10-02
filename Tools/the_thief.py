import requests
import json
import time

# Base URL for the API
BASE_URL = 'https://api.example.com'
API_KEY = 'your_api_key_here'

# Headers (e.g., for authentication or content-type)
headers = {
    'Authorization': f'Bearer {API_KEY}',
    'Content-Type': 'application/json'
}

# Function to perform GET request
def get_data(endpoint):
    url = f'{BASE_URL}/{endpoint}'
    response = requests.get(url, headers=headers)

    # Handle response
    if response.status_code == 200:
        return response.json()  # Parse JSON response
    else:
        print(f'Failed to retrieve data: {response.status_code}')
        return None

# Function to perform POST request
def post_data(endpoint, payload):
    url = f'{BASE_URL}/{endpoint}'
    response = requests.post(url, headers=headers, data=json.dumps(payload))

    # Handle response
    if response.status_code == 201:  # 201 means created successfully
        return response.json()  # Return the created resource
    else:
        print(f'Failed to post data: {response.status_code}')
        return None

# Function to perform PUT request
def put_data(endpoint, payload):
    url = f'{BASE_URL}/{endpoint}'
    response = requests.put(url, headers=headers, data=json.dumps(payload))

    # Handle response
    if response.status_code == 200:
        return response.json()
    else:
        print(f'Failed to update data: {response.status_code}')
        return None

# Function to perform DELETE request
def delete_data(endpoint):
    url = f'{BASE_URL}/{endpoint}'
    response = requests.delete(url, headers=headers)

    # Handle response
    if response.status_code == 204:  # 204 means no content (successful delete)
        print(f'Successfully deleted: {endpoint}')
    else:
        print(f'Failed to delete data: {response.status_code}')

# Example automation task - Fetch, create, update, and delete resources
def automation_task():
    # 1. Fetch data from API (GET)
    data = get_data('games')
    if data:
        print("Games data:", data)

    # 2. Create new resource (POST)
    new_game = {
        "name": "My New Game",
        "release_date": "2024-12-01",
        "background_image_url": "https://example.com/image.jpg"
    }
    created_game = post_data('games', new_game)
    if created_game:
        print("Created Game:", created_game)

    # 3. Update an existing resource (PUT)
    updated_game = {
        "name": "Updated Game Name",
        "release_date": "2024-12-15"
    }
    updated_game_result = put_data(f'games/{created_game["id"]}', updated_game)
    if updated_game_result:
        print("Updated Game:", updated_game_result)

    # 4. Delete a resource (DELETE)
    delete_data(f'games/{created_game["id"]}')

# Optional: Add a delay between tasks to prevent overloading the server
def run_with_delay(task, delay=5):
    while True:
        task()
        time.sleep(delay)

if __name__ == "__main__":
    # Run the automation task (without delay)
    automation_task()

    # Or run the task in intervals (e.g., every 5 seconds)
    # run_with_delay(automation_task, delay=5)
