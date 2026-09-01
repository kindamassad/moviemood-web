document.addEventListener('DOMContentLoaded', function() {
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }

    const csrfToken = document.querySelector('meta[name="csrf-token"]')?.content || '';

    function postForm(url, params) {
        const body = new URLSearchParams(params);
        return fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-CSRF-TOKEN': csrfToken
            },
            credentials: 'include',
            body
        });
    }

    function showToast(message, isSuccess = true) {
        const toast = document.createElement('div');
        toast.className = `toast ${isSuccess ? 'toast-success' : 'toast-error'}`;
        toast.textContent = message;
        document.body.appendChild(toast);
        
        setTimeout(() => {
            toast.classList.add('show');
        }, 10);
        
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => {
                document.body.removeChild(toast);
            }, 300);
        }, 3000);
    }

    async function checkMovieState(movieId, button) {
        try {
            const response = await postForm('/Lists/CheckMovie', { movieId });
            if (!response.ok) throw new Error('Network response was not ok');
            
            const data = await response.json();
            
            if (button.classList.contains('btn-add-to-list')) {
                button.classList.toggle('active', data.in_watchlist);
                button.title = data.in_watchlist ? 'Remove from list' : 'Add to list';
                button.innerHTML = `<i class="fas ${data.in_watchlist ? 'fa-check' : 'fa-plus'}"></i>`;
            } 
            else if (button.classList.contains('btn-add-to-favorites')) {
                button.classList.toggle('active', data.in_favorites);
                button.title = data.in_favorites ? 'Remove from favorites' : 'Add to favorites';
                button.innerHTML = '<i class="fas fa-heart"></i>';
            } 
            else if (button.classList.contains('btn-add-to-custom')) {
                button.classList.toggle('active', data.in_custom);
                button.title = 'Manage lists';
            }
        } catch (error) {
            console.error('Error checking movie state:', error);
        }
    }

    async function toggleMovieList(movieId, listType, listName = null) {
        const params = { movieId, listType };
        if (listName) params.listName = listName;
        
        try {
            const response = await postForm('/Lists/ToggleMovie', params);
            if (!response.ok) throw new Error('Network response was not ok');
            return await response.json();
        } catch (error) {
            console.error('Error:', error);
            return { success: false, message: 'Connection error' };
        }
    }

    async function showListModal(movieId) {
    if (!movieId || movieId <= 0) {
        showToast('Invalid movie ID', false);
        return;
    }

    const modal = document.createElement('div');
    modal.className = 'custom-list-modal';
    modal.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">Manage Lists</h3>
                <button class="close-modal-btn">&times;</button>
            </div>
            <div class="lists-container">Loading lists...</div>
            <div class="new-list-form">
                <input type="text" class="new-list-input" placeholder="New list name" required>
                <button class="create-list-btn">Create</button>
            </div>
        </div>
    `;

    document.body.appendChild(modal);
    document.body.style.overflow = 'hidden';

    modal.querySelector('.close-modal-btn').addEventListener('click', () => {
        document.body.removeChild(modal);
        document.body.style.overflow = '';
    });

    const listsContainer = modal.querySelector('.lists-container');
    const newListInput = modal.querySelector('.new-list-input');

    try {
        const [listsResponse, statusResponse] = await Promise.all([
            fetch('/Lists/GetLists', { credentials: 'include' }),
            postForm('/Lists/CheckMovie', { movieId })
        ]);

        if (!listsResponse.ok || !statusResponse.ok) {
            throw new Error('Request failed');
        }

        const [listsData, statusData] = await Promise.all([
            listsResponse.json(),
            statusResponse.json()
        ]);

        listsContainer.innerHTML = '';
        
        if (!listsData.lists || listsData.lists.length === 0) {
            listsContainer.innerHTML = '<p>No lists created yet</p>';
        } else {
            listsData.lists.forEach(list => {
                const isAdded = statusData.custom_lists?.includes(list) || false;
                const listOption = createListOption(list, isAdded, movieId);
                listsContainer.appendChild(listOption);
            });
        }

        modal.querySelector('.create-list-btn').addEventListener('click', async () => {
            const listName = newListInput.value.trim();
            if (!listName) {
                showToast('Please enter a list name', false);
                newListInput.focus();
                return;
            }

            const result = await toggleMovieList(movieId, 'custom', listName);
            if (result.success) {
                const existingList = Array.from(listsContainer.querySelectorAll('.list-option'))
                    .find(option => option.textContent.includes(listName));
                
                if (!existingList) {
                    const newListOption = createListOption(listName, true, movieId);
                    listsContainer.prepend(newListOption);
                }
                
                newListInput.value = '';
                showToast(result.message, true);
                
                document.querySelectorAll(`.btn-add-to-custom[data-movie-id="${movieId}"]`).forEach(btn => {
                    btn.classList.add('active');
                });
            } else {
                showToast(result.message || 'Failed to create list', false);
            }
        });

        newListInput.addEventListener('keypress', async (e) => {
            if (e.key === 'Enter') {
                modal.querySelector('.create-list-btn').click();
            }
        });

    } catch (error) {
        console.error('Error:', error);
        listsContainer.textContent = 'Failed to load lists';
    }
}

function createListOption(listName, isAdded, movieId) {
    const listOption = document.createElement('button');
    listOption.className = `list-option ${isAdded ? 'added' : ''}`;
    listOption.innerHTML = `
        <div class="list-content">
            <i class="fas ${isAdded ? 'fa-check list-check' : 'fa-list list-icon'}"></i>
            <span>${listName}</span>
        </div>
        ${isAdded ? '<button class="remove-btn"><i class="fas fa-times"></i></button>' : ''}
    `;

    listOption.addEventListener('click', async (e) => {
        if (e.target.closest('.remove-btn')) return;
        
        const result = await toggleMovieList(movieId, 'custom', listName);
        if (result.success) {
            const newIsAdded = result.action === 'added';
            listOption.className = `list-option ${newIsAdded ? 'added' : ''}`;
            listOption.innerHTML = `
                <div class="list-content">
                    <i class="fas ${newIsAdded ? 'fa-check list-check' : 'fa-list list-icon'}"></i>
                    <span>${listName}</span>
                </div>
                ${newIsAdded ? '<button class="remove-btn"><i class="fas fa-times"></i></button>' : ''}
            `;
            
            if (newIsAdded) {
                listOption.querySelector('.remove-btn').addEventListener('click', async (e) => {
                    e.stopPropagation();
                    const removeResult = await toggleMovieList(movieId, 'custom', listName);
                    if (removeResult.success) {
                        listOption.classList.remove('added');
                        listOption.innerHTML = `
                            <div class="list-content">
                                <i class="fas fa-list list-icon"></i>
                                <span>${listName}</span>
                            </div>
                        `;
                    }
                });
            }
            
            showToast(result.message, true);
        }
    });

    if (isAdded) {
        listOption.querySelector('.remove-btn').addEventListener('click', async (e) => {
            e.stopPropagation();
            const result = await toggleMovieList(movieId, 'custom', listName);
            if (result.success) {
                listOption.classList.remove('added');
                listOption.innerHTML = `
                    <div class="list-content">
                        <i class="fas fa-list list-icon"></i>
                        <span>${listName}</span>
                    </div>
                `;
                showToast(result.message, true);
            }
        });
    }

    return listOption;
}

    function setupButtons() {
        document.querySelectorAll('.btn-add-to-list, .btn-add-to-favorites, .btn-add-to-custom').forEach(button => {
            const movieId = button.dataset.movieId;
            if (!movieId) return;

            checkMovieState(movieId, button);

            button.addEventListener('click', async function(e) {
                e.preventDefault();
                e.stopPropagation();

                if (this.classList.contains('btn-add-to-custom')) {
                    showListModal(movieId);
                    return;
                }
                
                const listType = this.classList.contains('btn-add-to-list') ? 'watchlist' : 'favorites';
                const result = await toggleMovieList(movieId, listType);
                
                if (result.success) {
                    checkMovieState(movieId, this);
                    showToast(result.message, true);
                }
            });
        });
    }

    setupButtons();

    const trailerModal = document.getElementById('trailerModal');
    const trailerIframeContainer = document.getElementById('trailerIframeContainer');
    const watchTrailerBtn = document.getElementById('watchTrailerBtn');
    const closeTrailerBtn = document.querySelector('.close-trailer');

    if (watchTrailerBtn) {
        watchTrailerBtn.addEventListener('click', function() {
            if (this.classList.contains('disabled-btn') || this.disabled) return;
            
            const trailerKey = this.getAttribute('data-trailer-key');
            if (trailerKey) {
                trailerIframeContainer.innerHTML = `
                    <iframe src="https://www.youtube.com/embed/${trailerKey}?autoplay=1&rel=0" 
                            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" 
                            allowfullscreen></iframe>
                `;
                trailerModal.classList.add('active');
                document.body.style.overflow = 'hidden';
            }
        });
    }

    if (closeTrailerBtn) {
        closeTrailerBtn.addEventListener('click', function() {
            trailerModal.classList.remove('active');
            trailerIframeContainer.innerHTML = '';
            document.body.style.overflow = '';
        });
    }

    if (trailerModal) {
        trailerModal.addEventListener('click', function(e) {
            if (e.target === this) {
                this.classList.remove('active');
                trailerIframeContainer.innerHTML = '';
                document.body.style.overflow = '';
            }
        });
    }
});
