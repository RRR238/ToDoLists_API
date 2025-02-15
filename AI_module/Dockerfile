# Use official Python image as base
FROM python:3.11

# Set working directory inside container
WORKDIR /app

# Copy requirements and install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the entire app into the container
COPY . .

# Expose the port FastAPI will run on (match LOCAL_PORT)
EXPOSE 5000

# Set environment variables (for Docker Compose)
ENV SECRET_KEY="ExtremelyVeryStrongSecretKeyForJWT"
ENV ALGORITHM="HS256"
ENV ISSUER="http://localhost:7118"
ENV AUDIENCE="ToDoListsAPI"
ENV COLLECTION="to_do_list_collection"
ENV EMBEDDING_MODEL="sentence-transformers/all-MiniLM-L6-v2"
ENV VECTOR_SIZE="384"
ENV QDRANT_HOST="qdrant"
ENV LOCAL_PORT="5000"
ENV PYTHONUNBUFFERED="1"

# Run the app
CMD ["sh", "-c", "sleep 10 && python create_collection.py && python API.py"]
