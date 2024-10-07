import requests
import json
import time

# Base URL for the API
BASE_URL = 'https://api.rawg.io/api'


# Function to perform GET request
def get_data(endpoint,filters = ""):
    url = f'{BASE_URL}/{endpoint}?key=6b5fcc8fe6c748eaac97665e86118d6f{filters}'
    response = requests.get(url)

    # Handle response
    if response.status_code == 200:
        return response.json()  # Parse JSON response
    else:
        print(f'Failed to retrieve data: {response.status_code}')
        return None
def get_data(fullUrl):
  
    response = requests.get(fullUrl)

    # Handle response
    if response.status_code == 200:
        return response.json()  # Parse JSON response
    else:
        print(f'Failed to retrieve data: {response.status_code}')
        return None
post_header = {
    "Content-Type": "application/json"
}
# Function to perform POST request
def post_data(BaseUrl : str,endpoint, payload):
    url = f'{BaseUrl}/{endpoint}'
    print(url)
    print(payload)
    response = requests.post(url, data=json.dumps(payload),headers=post_header)

    # Handle response
    if response.status_code == 201:  # 201 means created successfully
        return response.json()  # Return the created resource
    else:
        print(f'Failed to post data: {response.status_code}')
        return None

# Function to perform PUT request
def put_data(endpoint, payload):
    url = f'{BASE_URL}/{endpoint}'
    response = requests.put(url, data=json.dumps(payload))

    # Handle response
    if response.status_code == 200:
        return response.json()
    else:
        print(f'Failed to update data: {response.status_code}')
        return None

post_base_url = "http://localhost:5291/api"
# Example automation task - Fetch, create, update, and delete resources
def automation_task():
    # 1. Fetch data from API (GET)
    i = 0
    next =  "https://api.rawg.io/api/games?key=6b5fcc8fe6c748eaac97665e86118d6f&metacritic=%2280-100%22&page_size=1000"
    while(i < 10000):
        data = get_data(next)
        results = data["results"]
        for result in results:
            dataToSend = dict()
            dataToSend['name'] = result["name"]
            if  dataToSend['name'] is None:
                continue
            dataToSend['releaseDate'] = result["released"]
            if  dataToSend['releaseDate'] is None:
                continue
            dataToSend['genreNames'] = [  genre["name"]  for genre in result["genres"]]
            if  dataToSend['genreNames'] is None:
                continue
            dataToSend['platformNames'] = [  platform["platform"]["name"]  for platform in result["platforms"]]
            if  dataToSend['platformNames'] is None:
                continue
            dataToSend['metaCritic'] = result['metacritic']
            if  dataToSend['metaCritic'] is None:
                continue
            dataToSend['backgroundImageUrl'] = result['background_image']
            if  dataToSend['backgroundImageUrl'] is None:
                continue
            post_data('http://localhost:5291/api','games', dataToSend)
            next = data["next"]
            i = i + 1
            time.sleep(0.001)
        
    
        
if __name__ == "__main__":
    # Run the automation task (without delay)
    automation_task()

    # Or run the task in intervals (e.g., every 5 seconds)
    # run_with_delay(automation_task, delay=5)
