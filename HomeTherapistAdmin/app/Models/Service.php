<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use App\Models\User;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;

class Service extends Model
{
    use CrudTrait;
    use HasFactory;

    public function therapists()
    {
        return $this->belongsToMany(User::class, 'therapist_open_services', 'services_id', 'user_id')
            ->using(TherapistOpenService::class);
    }
}
