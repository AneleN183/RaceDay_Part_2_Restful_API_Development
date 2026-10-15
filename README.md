💻Race Day Part 2 - RESTful API Development💻

✨A RESTful API built using ASP.NET Core for managing Race Day events, categories, and authentication. 

##📸SCREENSHOTS 

1. Solution Explorer Structure:
   <img width="503" height="1011" alt="image" src="https://github.com/user-attachments/assets/0970482c-670f-40ce-8e21-7b858e1a4e9b" />
* Please note that a  Unit Testing project was implemented but does not display here on GitHub due to Visual Studio configuration. However, the tests are done. Proof will be shown of that if possible.  

🧪Unit Testing🧪
1. Unit Test #1 in userControllerTests.cs:  
<img width="474" height="200" alt="image" src="https://github.com/user-attachments/assets/7982ac95-2d48-421c-8d0a-0952ad536f85" /

💻Code in usersControllerTests.cs:
<img width="1757" height="519" alt="image" src="https://github.com/user-attachments/assets/607ab857-ac07-45b9-8dfb-324cd74176fe" />


These are the results for usersControllerTests.cs: 
✨Explanation✨: The GET/api/users/me endpoint returns the current user's profile when the user exists. This verifies that the endpoint correctly retrieves the current user. 
<img width="1910" height="1075" alt="image" src="https://github.com/user-attachments/assets/44e394f0-5150-4fca-9c3c-4e0a9c68f398" />

Unit Test #2 in usersControllerTests.cs:

💻Code:
<img width="1914" height="336" alt="image" src="https://github.com/user-attachments/assets/801f8077-4b9f-4b93-9977-5eb44675f381" />


These are the results for second unit test in usersControllerTests.cs
✨Explanation:✨This test tests that the same endpoint returns 404 Not Found when the user doesn't exist
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/dc068476-f69b-46d9-a0c2-b7e06d6d5fb5" />


2. Running tests in eventsControllerTests.cs:
   <img width="475" height="143" alt="image" src="https://github.com/user-attachments/assets/6f8d317a-04a7-44ba-b7b2-562dd5624247" />


3. Unit Test #1 in categoriesControllerTests.cs:
   <img width="479" height="194" alt="image" src="https://github.com/user-attachments/assets/1ff4117a-3deb-4472-8154-e0240727e613" />

  💻Code in categoriesControllerTests.cs
  <img width="1911" height="1073" alt="image" src="https://github.com/user-attachments/assets/e7c4e147-e7ee-4272-af3d-c13babc603b5" />
  
These are the results for first unit test in categoriesControllerTests.cs
  ✨Explanation✨: It tests that the endpoint correctly returns all categories from the database 
  <img width="1915" height="1072" alt="image" src="https://github.com/user-attachments/assets/23931ea6-2549-4b8f-b964-18ab346b2b4f" />

  Unit Test #2 in categoriesControllerTests.cs:

  💻Code in categoriesControllerTests.cs:
  <img width="1894" height="439" alt="image" src="https://github.com/user-attachments/assets/c3f1327d-ba3e-454c-a3be-900bc8a4bd62" />

  These are the results for the second unit test in categoriesControllerTests.cs
  ✨Explanation✨: It tests that the endpoint correctly returns a single category when the requested ID exists
  <img width="1913" height="1010" alt="image" src="https://github.com/user-attachments/assets/1a5013dd-e004-496a-82b7-e298639c708d" />

  Unit Test #3 in categoriesControllerTests.cs

  💻Code in categoriesContollerTests.cs:
  <img width="1889" height="528" alt="image" src="https://github.com/user-attachments/assets/0db7e596-d7f4-4226-8c60-349af691a552" />

  These are the results for the third unit test in categoriesControllerTests.cs
  ✨Explanation✨: It tests that the endpoint correctly returns a 404 Not Found when the requested category does not exist
  <img width="1918" height="1006" alt="image" src="https://github.com/user-attachments/assets/92e4ad93-1b8c-49be-b6bc-8edc44976762" />







